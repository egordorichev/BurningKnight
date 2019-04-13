using BurningKnight.assets;
using Lens;
using Lens.entity;
using Lens.graphics;

namespace BurningKnight.ui {
	public class SaveIndicator : Entity {
		private TextureRegion region;

		public override void Init() {
			base.Init();
			region = CommonAse.Ui.GetSlice("save");

			X = Display.Width - region.Width * 2;
			Y = Display.Height - region.Height * 2;
		}

		public override void Render() {			
			Graphics.Render(region, Position);
		}
	}
}