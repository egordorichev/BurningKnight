using System;
using System.Collections.Generic;
using System.Text;
using BurningKnight.ui.str.effect;
using BurningKnight.ui.str.@event;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Microsoft.Xna.Framework;
using MonoGame.Extended;
using MonoGame.Extended.BitmapFonts;

namespace BurningKnight.ui.str {
	public delegate void StartedTyping(UiString str);
	public delegate void FinishedTyping(UiString str);
	public delegate void CharTyped(UiString str, int i, char c);
	public delegate void EventFired(UiString str, string id);
	
	/*
	 * todo:
	 * [tg a]test[dl a]
	 * variables
	 */
	public class UiString : Entity {
		private string label;
		private BitmapFont font;
		private List<Glyph> glyphs = new List<Glyph>();
		private List<GlyphEffect> effects = new List<GlyphEffect>();
		private List<StrRenderer> renderers = new List<StrRenderer>();
		private float progress;
		private int lastChar;

		public float Delay;
		public bool Paused;
		public float Speed = 1f;
		public int WidthLimit;
		public bool Finished => progress >= glyphs.Count;

		public StartedTyping StartedTyping;
		public FinishedTyping FinishedTyping;
		public EventFired EventFired;
		public CharTyped CharTyped;
		public Action<Vector2, int> Renderer;

		public Color Tint;
		
		public string Label {
			get => label;
			set {
				label = value;
				Recalculate();
			}
		}

		public UiString(BitmapFont font) {
			this.font = font;

			Width = 4;
			Height = 4;
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

							case "rn": {
								try {
									renderers.Add(new StrRenderer {
										Id = parts.Length > 1 ? int.Parse(parts[1]) : 0,
										Where = builder.Length
									});
								} catch (Exception ex) {
									Log.Error(ex);
								}
								
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
			builder.Clear();
			var glp = font.GetGlyphs(label);

			if (WidthLimit > 0) {
				var k = 0;
				var lastSpace = 0;
				var sinceLastSpace = 0;
				var sinceLast = 0;
				var width = 0;
				var first = true;
				var spaceWidth = (int) (font.MeasureString("a a").Width - font.MeasureString("aa").Width);
				
				foreach (var g in glp) {
					var c = label[k];
					var w = 0;

					if (c == ' ') {
						lastSpace = k;
						sinceLastSpace = 0;
						w = spaceWidth;
					} else if (c == '\n') {
						sinceLast = 0;
						sinceLastSpace = 0;
					} else {
						w = g.FontRegion.Width;
					}

					if (c != '\n') {
						sinceLast += w;
						width += w;

						if (c != ' ') {
							sinceLastSpace += w;
						}
					}

					builder.Append(c);
					
					if (width >= WidthLimit) {
						if (first) {
							WidthLimit = width - sinceLastSpace - spaceWidth;
							first = false;
						}

						width -= sinceLast;
						
						sinceLast = width;
						builder[lastSpace] = '\n';
					}

					k++;
				}

				label = builder.ToString();
			}
			
			glp = font.GetGlyphs(label);
			
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
			if (label == null || Tint.A == 0 || glyphs.Count == 0) {
				return;
			}

			var m = 1;
			var n = m - 1;
			var l = (int) Math.Min(progress + n, glyphs.Count);

			for (var i = 0; i < l; i++) {
				var g = glyphs[i];
				
				if (g.G.FontRegion != null) {
					var pos = new Vector2(
						Position.X + g.G.Position.X + g.Offset.X + g.Origin.X,
						Position.Y + g.G.Position.Y + g.Offset.Y + g.Origin.Y
					);

					var c = g.Color;

					if (progress > i && progress < i + 1) {
						float v = (progress + n - i) / m;
						
						pos.Y -= (1 - v) * 8f;
						c.A = (byte) (v * 255);
					}

					c.A = (byte) MathUtils.Clamp(0, 255, c.A * Tint.A / 255f);

					Graphics.Batch.Draw(g.G.FontRegion.TextureRegion.Texture, pos,
						g.G.FontRegion.TextureRegion.Bounds, c, g.Angle, g.Origin, g.Scale, g.Effects, 0);
				}

				if (renderers.Count > 0) {
					foreach (var r in renderers) {
						if (r.Where == i) {
							Renderer(Position + g.G.Position + g.Offset - new Vector2(0, 9), r.Id);
						}
					}
				}
			}

			var gl = glyphs[Math.Min((int) progress + 1, glyphs.Count - 1)];

			if (gl.G.FontRegion != null) {
				Width = Math.Max(Width, gl.G.Position.X + gl.G.FontRegion.Width);
				Height = Math.Max(Height, gl.G.Position.Y + gl.G.FontRegion.Height);	
			}
		}

		public void FinishTyping() {
			progress = glyphs.Count;
			FinishedTyping?.Invoke(this);
		}

		public void StartTyping() {
			progress = 0f;
			lastChar = 0;
			Delay = 0;
			Paused = false;
			Speed = 1;

			if (WidthLimit > 0) {
				WidthLimit = 200;
			}
			
			StartedTyping?.Invoke(this);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Delay > 0 || Paused) {
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
					}

					if ((int) Math.Ceiling(progress) == glyphs.Count) {
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
				
				for (var j = e.Start; j < Math.Min(glyphs.Count - 1, e.End); j++) {
					e.Apply(glyphs[j], j);
				}
				
				if (e.Ended()) {
					effects.RemoveAt(i);
				}
			}
		}
	}
}