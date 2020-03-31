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
		public bool Tints = true;
		public bool Hide;

		protected override void OnHover() {
			if (Hide) {
				return;
			}

			base.OnHover();
			
			PlaySfx("ui_moving");

			if (Clickable) {
				Tween.To(1, Tint, x => Tint = x, 0.1f);
			}
		}

		protected override void OnUnhover() {
			if (Hide) {
				return;
			}

			base.OnUnhover();

			if (Clickable) {
				Tween.To(DefaultTint, Tint, x => Tint = x, 0.1f);
			}
		}

		public override void OnClick() {
			if (Hide) {
				return;
			}

			base.OnClick();
			Tint = 0.5f;

			if (Clickable) {
				Tween.To(1, Tint, x => Tint = x, 0.2f);
			}
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
			if (Hide) {
				return;
			}
			
			if (Tints) {
				var t = Tint + ((Tint - DefaultTint) * ((float) Math.Cos(Engine.Time * 17) * 0.5f - 0.5f) * 0.5f);
				Graphics.Color = new Color(t, t, t, 1f);
			}

			Graphics.Print(label, Font, Position + origin, angle, origin, new Vector2(scale));
			Graphics.Color = ColorUtils.WhiteColor;
		}
	}
}