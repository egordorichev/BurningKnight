using System;
using Lens.assets;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class UiButton : UiLabel {
		public Action Click;
		public ButtonType Type = ButtonType.Normal;
		public float Padding;

		public override bool CheckCollision(Vector2 v) {
			return new Rectangle((int) (X - Padding), (int) (Y - Padding), (int) (Width + Padding * 2), (int) (Height + Padding * 2)).Contains(v);
		}

		protected override void OnHover() {
			base.OnHover();
			Audio.PlaySfx("moving");
		}

		protected override void OnUnhover() {
			base.OnUnhover();
		}

		protected override void OnClick() {
			base.OnClick();
			Click?.Invoke();

			Audio.PlaySfx(Type == ButtonType.Normal ? "select" : "exit");
		}
	}
}