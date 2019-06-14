using System;
using Lens.assets;

namespace BurningKnight.ui {
	public class UiButton : UiLabel {
		public Action Click;
		public ButtonType Type = ButtonType.Normal;
		
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