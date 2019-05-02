using System;
using System.Collections.Generic;
using System.Text;
using BurningKnight.ui.str.effect;
using BurningKnight.ui.str.@event;
using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;

namespace BurningKnight.ui.str {
	public delegate void StartedTyping(UiString str);
	public delegate void FinishedTyping(UiString str);
	public delegate void CharTyped(UiString str, int i, char c);
	public delegate void EventFired(UiString str, string id);
	
	/*
	 * todo:
	 * [tg a]test[dl a]
	 */
	public class UiString : Entity {
		
		private string label;
		private BitmapFont font;
		private List<Glyph> glyphs = new List<Glyph>();
		private List<GlyphEffect> effects = new List<GlyphEffect>();
		private float progress;
		private int lastChar;

		public float Delay;
		public float Speed = 1f;
		public bool Finished => progress >= glyphs.Count;

		public StartedTyping StartedTyping;
		public FinishedTyping FinishedTyping;
		public EventFired EventFired;
		public CharTyped CharTyped;
		
		public string Label {
			get => label;
			set {
				if (label != value) {
					label = value;
					Recalculate();
				}
			}
		}

		public UiString(BitmapFont font) {
			this.font = font;

			AlwaysActive = true;
			AlwaysVisible = true;
		}

		private GlyphEffect FindEffect<T>(bool open = true) where T: GlyphEffect {
			var t = typeof(T);

			foreach (var e in effects) {
				if (e.GetType() == t && (!open || !e.Closed)) {
					return e;
				}
			}

			return null;
		}

		private void AddEffect<T>(StringBuilder builder) where T: GlyphEffect {
			var e = FindEffect<T>();

			if (e != null) {
				e.End = builder.Length;
				e.Closed = true;
			} else {
				var ef = (GlyphEffect) Activator.CreateInstance(typeof(T));

				ef.Start = builder.Length;
				effects.Add(ef);
			}
		}

		private void Recalculate() {
			glyphs.Clear();
			effects.Clear();
			
			StartTyping();

			var builder = new StringBuilder();
			var token = new StringBuilder();
			var lc = '\0';
			var parsingToken = false;
			var events = new List<GlyphEvent>();
			
			for (var i = 0; i < label.Length; i++) {
				var c = label[i];

				if (parsingToken) {
					if (c == ']') {
						parsingToken = false;
						
						var t = token.ToString().TrimStart().TrimEnd();
						var parts = t.Split(null);

						if (parts.Length == 0) {
							continue;
						}

						GlyphEvent e = null;

						switch (parts[0]) {
							case "sp": {
								e = new SpeedEvent();
								break;
							}

							case "dl": {
								e = new DelayEvent();
								break;
							}

							case "ev": {
								e = new UserEvent();
								break;
							}

							case "cl": {
								if (parts.Length >= 2) {
									var ef = new ColorEffect();

									ef.ParseColor(parts[1]);
									ef.Start = builder.Length;
									effects.Add(ef);
								} else {
									var en = FindEffect<ColorEffect>();

									if (en != null) {
										en.End = builder.Length;
										en.Closed = true;
									}
								}
								
								break;
							}

							case "/cl": {
								AddEffect<WaveEffect>(builder);
								break;
							}
						}

						if (e != null) {
							e.I = builder.Length;
							e.Parse(parts);
							
							events.Add(e);
						}
						
						continue;
					}

					token.Append(c);
					continue;
				}
				
				switch (c) {
					case '\\': {
						i++;
						break;
					}

					case '[': {
						parsingToken = true;
						token.Clear();
						break;
					}

					case '^': {
						if (lc == '^') {
							AddEffect<WaveEffect>(builder);
						}
						
						break;
					}

					case '*': {
						if (lc == '*') {
							AddEffect<BoldEffect>(builder);
						}
						
						break;
					}

					case '_': {
						AddEffect<ItalicEffect>(builder);
						break;
					}

					case '%': {
						if (lc == '%') {
							AddEffect<RainbowEffect>(builder);
						}
						
						break;
					}

					case '#': {
						if (lc == '#') {
							AddEffect<ShakeEffect>(builder);
						}
						
						break;
					}

					default: {
						builder.Append(c);
						break;
					}
				}

				lc = c;
			}

			foreach (var e in effects) {
				if (!e.Closed) {
					e.End = builder.Length;
				}
			}
			
			label = builder.ToString();
			
			var glp = font.GetGlyphs(label);
			var j = 0;

			foreach (var g in glp) {
				var gl = new Glyph {
					G = g
				};

				gl.Reset();
				glyphs.Add(gl);

				for (var i = events.Count - 1; i >= 0; i--) {
					var e = events[i];

					if (e.I == j) {
						gl.Events.Add(e);
						events.RemoveAt(i);
					}
				}

				j++;
			}
		}

		public override void Render() {
			if (label == null) {
				return;
			}

			var m = 1;
			var n = m - 1;

			for (var i = 0; i < Math.Min(progress + n, glyphs.Count); i++) {
				var g = glyphs[i];
				
				if (g.G.FontRegion != null) {
					var pos = new Vector2(
						Position.X + g.G.Position.X + g.Offset.X + g.Origin.X,
						Position.Y + g.G.Position.Y + g.Offset.Y + g.Origin.Y
					);

					var c = g.Color;

					if (progress < i + 1) {
						float v = (progress + n - i) / m;
						
						pos.Y -= (1 - v) * 8f;
						c.A = (byte) (v * 255);
					}
					
					Graphics.Batch.Draw(g.G.FontRegion.TextureRegion.Texture, pos,
						g.G.FontRegion.TextureRegion.Bounds, c, g.Angle, g.Origin, g.Scale, g.Effects, 0);

					if (progress < i + 1) {
						c.A = 255;
					}
				}
			}
		}

		public void FinishTyping() {
			if (Finished) {
				return;
			}
			
			progress = glyphs.Count;
			FinishedTyping?.Invoke(this);
		}

		public void StartTyping() {
			progress = 0f;
			lastChar = 0;
			Delay = 0;
			
			StartedTyping?.Invoke(this);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Delay > 0) {
				Delay -= dt;
			} else {
				progress = Math.Min(progress + Speed * dt * 20f, glyphs.Count);
				var v = (int) Math.Floor(progress);
				
				if (v > lastChar) {
					lastChar = v;

					if (v < glyphs.Count) {
						var g = glyphs[v];

						foreach (var e in g.Events) {
							e.Fire(this, g);
						}

						CharTyped?.Invoke(this, v, label[v]);
					} else {
						FinishTyping();
					}
				}
			}
			
			foreach (var g in glyphs) {
				g.Reset();
			}

			for (var i = effects.Count - 1; i >= 0; i--) {
				var e = effects[i];
				e.Update(dt);
				
				for (var j = e.Start; j < e.End; j++) {
					e.Apply(glyphs[j], j);
				}
				
				if (e.Ended()) {
					effects.RemoveAt(i);
				}
			}
		}
	}
}