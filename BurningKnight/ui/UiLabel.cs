using System;
using Lens;
using Lens.assets;
using Lens.graphics;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using MonoGame.Extended.BitmapFonts;

namespace BurningKnight.ui {
	public class UiLabel : UiEntity {
		public const float DefaultTint = 0.7f;
		
		protected string label;
		public BitmapFont Font = assets.Font.Medium;
		public float Tint = DefaultTint;

		protected override void OnHover() {
			base.OnHover();

			Audio.PlaySfx("moving", 0.5f);
			Tween.To(1, Tint, x => Tint = x, 0.1f);
		}

		protected override void OnUnhover() {
			base.OnUnhover();
			Tween.To(DefaultTint, Tint, x => Tint = x, 0.1f);
		}

		public override void OnClick() {
			base.OnClick();
			Tint = 0.5f;
			Tween.To(1, Tint, x => Tint = x, 0.2f);
		}

		public string Label {
			get => label;

			set {
				if (label != value) {
					label = value;
					
					var size = Font.MeasureString(label);

					Width = size.Width;
					Height = size.Height;
					
					origin = new Vector2(Width / 2, Height / 2);
				}
			}
		}

		public string LocaleLabel {
			set => Label = Locale.Get(value);
		}

		public override void Render() {
			var t = Tint + ((Tint - DefaultTint) * ((float) Math.Cos(Engine.Time * 17) * 0.5f - 0.5f) * 0.5f);
			
			Graphics.Color = new Color(t, t, t, 1f);
			Graphics.Print(label, Font, Position + origin, angle, origin, new Vector2(scale));
			Graphics.Color = ColorUtils.WhiteColor;
		}
	}
}