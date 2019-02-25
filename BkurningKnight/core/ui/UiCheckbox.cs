using BurningKnight.core.assets;

namespace BurningKnight.core.ui {
	public class UiCheckbox : UiChoice {
		public UiCheckbox(string Label, int X, int Y) {
			base(Label, X, Y);
			this.SetChoices({ "on", "off" });
		}

		public UiCheckbox SetOn(bool On) {
			this.SetCurrent(On ? 0 : 1);

			return this;
		}

		public bool IsOn() {
			return GetCurrent() == 0;
		}

		protected override Void SetColor() {
			if (IsOn()) {
				Graphics.Medium.SetColor(0.2f, 0.8f, 0.2f, 1f);
			} else {
				Graphics.Medium.SetColor(0.8f, 0.2f, 0.2f, 1f);
			}

		}
	}
}
