using System;

namespace BurningKnight.ui {
	public class UiCheckbox : UiChoice {
		public bool On {
			get => Option == 0;
			set {
				if (On != value) {
					Option = value ? 0 : 1;
				}
			}
		}

		public UiCheckbox() {
			Options = new[] {
				"on", "off"
			};
		}
	}
}