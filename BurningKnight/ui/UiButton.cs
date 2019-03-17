using System;

namespace BurningKnight.ui {
	public class UiButton : UiLabel {
		public Action Click;
		
		protected override void OnHover() {
			base.OnHover();
		}

		protected override void OnUnhover() {
			base.OnUnhover();
		}

		protected override void OnClick() {
			base.OnClick();
			Click?.Invoke();
		}
	}
}