using System;
using BurningKnight.assets;
using Lens.entity;
using Lens.graphics;
using Lens.input;
using Lens.util;
using Microsoft.Xna.Framework;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace BurningKnight.state {
	public class PhotoCard : Entity {
		private static Color tint = new Color(0.6f, 0.6f, 0.6f, 1f);
		
		public TextureRegion Region;
		public string Name;
		public string Tasks;
		public string Url;
		public Vector2 Target;
		public float Angle;
		public bool GoAway;

		private Vector2 start;
		private float t;
		private bool doneLerping;
		private bool hovered;
		private bool wasHovered;
		private float sc = 1;
		public float Scale = 0.6f;
		
		public override void Init() {
			base.Init();

			start = Position;
			AlwaysActive = true;
			AlwaysVisible = true;
			Width = Region.Width;
			Height = Region.Height;
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			t += dt;

			if (Math.Abs(Angle) > 0.1f) {
				Angle -= Angle * (dt * 2.5f);
			}

			if (GoAway) {
				var dx = start.X - Position.X;
				var dy = start.Y - Position.Y;
				var d = MathUtils.Distance(dx, dy);

				var s = dt;
				
				X += dx * s;
				Y += dy * s;

				if (d < 0.3f) {
					Done = true;
				}
			} else if (!doneLerping) {
				var dx = Target.X - Position.X;
				var dy = Target.Y - Position.Y;
				var d = MathUtils.Distance(dx, dy);

				var s = dt * 4f;
				
				X += dx * s;
				Y += dy * s;

				if (d < 0.3f) {
					Position = Target;
					doneLerping = true;
				}
			}

			var w = Width * Scale;
			var h = Height * Scale;

			wasHovered = hovered;
			hovered = new Rectangle((int) (X - w / 2f), (int) (Y - h / 2f), (int) w, (int) h).Contains(Input.Mouse.UiPosition);

			sc += ((hovered ? 1.3f : 1) - sc) * dt * 5;

			if (hovered) {
				if (Input.Mouse.WasPressedLeftButton) {
					Web.Open(Url);
				}
			}
		}

		public override void Render() {
			var scl = Scale * sc;
			var a = Math.Max(0, Math.Abs(Angle) - 0.1f) * Math.Sign(Angle);

			Graphics.Render(Region, Position, Angle, Region.Center, new Vector2(scl));

			if (Name != null) {
				Print(Name, (int) Position.X, (int) (Position.Y + Height / 2f * scl + 15), a);
				Graphics.Color = tint;
			}

			if (Tasks != null) {
				Print(Tasks, (int) Position.X, (int) (Position.Y + Height / 2f * scl + 25), a);
				Graphics.Color = ColorUtils.WhiteColor;
			}
		}

		private static void Print(string s, int x, int y, float angle) {
			var m = Font.Small.MeasureString(s);
			Graphics.Print(s, Font.Small, new Vector2(x, y), angle, new Vector2(m.Width / 2f, m.Height / 2f), Vector2.One);
		}
	}
}