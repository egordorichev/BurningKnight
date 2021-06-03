using System;
using System.Collections.Generic;
using System.Text;
using BurningKnight.ui.str.effect;
using BurningKnight.ui.str.@event;
using Lens;
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
	 * syntax:
	 *
	 * _ starts italic
	 * ** starts bold
	 * ## starts shake
	 * @@ starts blink
	 * && starts flip
	 * %% starts rainbow
	 * ^^ starts wave
	 * ~~ starts randomizer ~~
	 *
	 * [event_name var1 var2] starts an event
	 *  + [dl time] delays
	 *  + [skp] finishes printing out the string
	 *  + [sp speed] sets speed
	 *  + [ev event] fires user event
	 *  + [vr variable_name] replaced with user variable
	 *  + [cl color] sets color, can be hex or predefined string in Palette class
	 *  + [ic id] draws an icon with id=id
	 */
	
	/*
	 * todo:
	 * [tg a]test[dl a]
	 */
	public class UiString : Entity {
		private string label;
		private BitmapFont font;
		private List<Glyph> glyphs = new List<Glyph>();
		private List<GlyphEffect> effects = new List<GlyphEffect>();
		private List<StrRenderer> renderers = new List<StrRenderer>();
		private float progress;
		private int lastChar;
		public float FinalWidth;
		private float finalHeight;

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
		public Dictionary<string, object> Variables = new Dictionary<string, object>();
		public List<TextureRegion> Icons = new List<TextureRegion>();

		public void SetVariable(string id, object o) {
			Variables[id] = o;
		}

		public void ClearIcons() {
			Icons.Clear();
		}
		
		public void AddIcon(TextureRegion o) {
			if (o == null) {
				Log.Error("Unknown icon");
			}
			
			Icons.Add(o);
		}

		public Color Tint = Color.White;
		
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
			renderers.Clear();
			
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
							case "skp": {
								e = new SkipEvent();
								break;
							}

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

							case "vr": {
								if (parts.Length > 1) {
									if (Variables.TryGetValue(parts[1], out var vr)) {
										builder.Append(vr == null ? "null" : vr.ToString());
									} else {
										Log.Error($"Undefined variable {parts[1]}!");
									}

									break;
								}
								
								continue;
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

							case "ic": {
								try {
									renderers.Add(new IconRenderer {
										Id = parts.Length > 1 ? int.Parse(parts[1]) : 0,
										Where = builder.Length
									});
									
									builder.Append(' ');
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
					case '[': {
						if (lc == '\\') {
							builder.Remove(builder.Length - 1, 1);
							builder.Append('[');
							break;
						}
						
						parsingToken = true;
						token.Clear();
						break;
					}

					case '^': {
						if (lc == '^') {
							builder.Remove(builder.Length - 1, 1);
							AddEffect<WaveEffect>(builder);
						} else {
							builder.Append(c);
						}
						
						break;
					}

					case '*': {
						if (lc == '*') {
							builder.Remove(builder.Length - 1, 1);
							AddEffect<BoldEffect>(builder);
						} else {
							builder.Append(c);
						}

						break;
					}

					case '_': {
						if (lc == '\\') {
							builder.Remove(builder.Length - 1, 1);
							builder.Append(c);
						} else {
							AddEffect<ItalicEffect>(builder);
						}

						break;
					}

					case '%': {
						if (lc == '%') {
							builder.Remove(builder.Length - 1, 1);
							AddEffect<RainbowEffect>(builder);
						} else {
							builder.Append(c);
						}
						
						break;
					}
					
					case '&': {
						if (lc == '&') {
							builder.Remove(builder.Length - 1, 1);
							AddEffect<FlipEffect>(builder);
						} else {
							builder.Append(c);
						}

						continue;
					}

					case '@': {
						if (lc == '@') {
							builder.Remove(builder.Length - 1, 1);
							AddEffect<BlinkEffect>(builder);
						} else {
							builder.Append(c);
						}
						
						break;
					}

					case '#': {
						if (lc == '#') {
							builder.Remove(builder.Length - 1, 1);
							AddEffect<ShakeEffect>(builder);
						} else {
							builder.Append(c);
						}
						
						break;
					}

					case '~': {
						if (lc == '~') {
							builder.Remove(builder.Length - 1, 1);
							AddEffect<RandomEffect>(builder);
						} else {
							builder.Append(c);
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
			var spaceWidth = (int) (font.MeasureString("a a").Width - font.MeasureString("aa").Width);

			if (WidthLimit > 0) {
				var k = 0;
				var lastSpace = 0;
				var sinceLastSpace = 0;
				var sinceLast = 0;
				var width = 0;
				var first = true;
				var i = 0;
				
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
						width = 0;
					} else {
						w = g.FontRegion.Width;
					}

					var hadIcon = false;
					
					if (renderers.Count > 0) {
						foreach (var r in renderers) {
							if (r.Where == i) {
								if (r is IconRenderer ir) {
									w += ir.GetWidth(this) - spaceWidth;
									hadIcon = true;
								}
							}
						}
					}
					
					i++;

					if (c != '\n' || hadIcon) {
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
			var size = font.MeasureString(label);

			FinalWidth = size.Width;
			finalHeight = size.Height - 4;

			var j = 0;
			var ww = 0;

			foreach (var g in glp) {
				var gl = new Glyph {
					G = g
				};
				
				if (label[j] == '\n') {
					ww = 0;
				}
				
				gl.G.Position.X += ww;
				
				if (renderers.Count > 0) {
					foreach (var r in renderers) {
						if (r.Where == j) {
							if (r is IconRenderer ir) {
								var v = ir.GetWidth(this) - spaceWidth;
								ww += v;
								FinalWidth += v;
							}
						}
					}
				}

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
		
		public void Stop() {
			Paused = true;
		}
		
		public bool ShouldntRender => Engine.Instance.State.Paused || label == null || Tint.A == 0 || glyphs.Count == 0;
		public bool DisableRender;

		public override void Render() {
			if (ShouldntRender || DisableRender) {
				return;
			}

			RenderString();
		}

		public void RenderString() {
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

					c.A = (byte) MathUtils.Clamp(0, 255, (float) c.A * Tint.A / 255f);

					Graphics.Batch.Draw(g.G.FontRegion.TextureRegion.Texture, pos,
						g.G.FontRegion.TextureRegion.Bounds, c, g.Angle, g.Origin, g.Scale, g.Effects, 0);
				}

				if (renderers.Count > 0) {
					Graphics.Color.A = (byte) MathUtils.Clamp(0, 255, (float) g.Color.A * Tint.A / 255f);

					foreach (var r in renderers) {
						if (r.Where == i) {
							if (r is IconRenderer ir) {
								if (ir.Region != null) {
									Graphics.Render(ir.Region, new Vector2(Position.X + g.G.Position.X + g.Offset.X + g.Origin.X,
										Position.Y + g.G.Position.Y + (ir.Region.Height - 7) / 2f - ir.Region.Height + 1 + g.Offset.Y + g.Origin.Y));
								}
							} else {
								Renderer(Position + g.G.Position + g.Offset - new Vector2(0, 9), r.Id);
							}
						}
					}
				}
				
				Graphics.Color.A = 255;
			}
		}

		public void FinishTyping() {
			progress = glyphs.Count;
			Width = FinalWidth;
			Height = finalHeight;
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
				progress = Math.Min(progress + Speed * dt * 25f, glyphs.Count);
				var v = (int) Math.Floor(progress);

				while (true) {
					if (v > lastChar) {
						lastChar++;

						if (v < glyphs.Count) {
							var g = glyphs[v];

							foreach (var e in g.Events) {
								e.Fire(this, g);
							}

							CharTyped?.Invoke(this, v, label[v]);

							if (g.G.FontRegion != null) {
								Width = Math.Max(Width, g.G.Position.X + g.G.FontRegion.Width);
								Height = Math.Max(Height, g.G.Position.Y + g.G.FontRegion.Height);
							}
						}

						if ((int) Math.Ceiling(progress) == glyphs.Count) {
							FinishTyping();
						}
					} else {
						break;
					}
				}
			}
			
			foreach (var g in glyphs) {
				g.Reset();
			}

			for (var i = effects.Count - 1; i >= 0; i--) {
				var e = effects[i];
				e.Update(dt);
				
				for (var j = e.Start; j < Math.Min(glyphs.Count, e.End); j++) {
					e.Apply(glyphs[j], j);
				}
				
				if (e.Ended()) {
					effects.RemoveAt(i);
				}
			}
		}
	}
}