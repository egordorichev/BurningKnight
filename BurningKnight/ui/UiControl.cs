using BurningKnight.assets.input;
using Lens.assets;

namespace BurningKnight.ui {
	public class UiControl : UiButton {
		public string Key;

		public override void Init() {
			base.Init();
			Label = $"{Locale.Get(Key)}: {Controls.Find(Key)}";
		}
	}
}