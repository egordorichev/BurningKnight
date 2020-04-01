using System;
using BurningKnight.assets;
using Lens;
using Lens.graphics;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class UiTableEntry : UiButton {
		private const float XPadding = 3;
		public const float YPadding = 1;
		
		private TextureRegion texture;
		private string value;
		private float valueWidth;
		private Vector2 textOrigin;
		private Vector2 valueOrigin;

		public Color Color = ColorUtils.WhiteColor;

		public string Value {
			set {
				this.value = value;

				valueWidth = Font.MeasureString(this.value).Width;
			}
		}

		public override void OnClick() {
			base.OnClick();
			Click?.Invoke(this);
		}

		public override void AddComponents() {
			base.AddComponents();

			texture = CommonAse.Ui.GetSlice("table_item");
			Width = texture.Width;
			Height = texture.Height;

			Font = assets.Font.Small;
		}

		public override void PostInit() {
			base.PostInit();
			
			if (Click == null) {
				RemoveTag(Tags.Button);
				Clickable = false;
				ScaleMod = 0;
			}

			textOrigin = new Vector2(Width / 2f, Height / 2f);
			valueOrigin = new Vector2(-Width / 2f + valueWidth, Height / 2f);
		}

		protected override void OnHover() {
			base.OnHover();

			if (Clickable) {
				Tween.To(1 + ScaleMod * 0.2f, Depth, x => Depth = (int) x, 0.1f);
			}
		}

		protected override void OnUnhover() {
			base.OnUnhover();

			if (Clickable) {
				Tween.To(1f, Depth, x => Depth = (int) x, 0.1f);
			}
		}

		public override void Render() {
			var or = texture.Center;
			var s = new Vector2(scale);
			
			Graphics.Color.A = 220;
			Graphics.Render(texture, Position + or, angle, or, s);
			Graphics.Color.A = 255;

			if (Clickable) {
				var t = Tint + ((Tint - DefaultTint) * ((float) Math.Cos(Engine.Time * 17) * 0.5f - 0.5f) * 0.5f);
				Graphics.Color = new Color(Color.R / 255f * t, Color.G / 255f * t, Color.B / 255f * t, Color.A / 255f);
			} else {
				Graphics.Color = Color;
			}

			Graphics.Print(label, Font, new Vector2(X + XPadding, Y + YPadding) + textOrigin, angle, textOrigin, s);
			Graphics.Print(value, Font, new Vector2(Right - valueWidth + XPadding, Y + YPadding) + valueOrigin, angle, valueOrigin, s);
			Graphics.Color = ColorUtils.WhiteColor;
		}
	}
}