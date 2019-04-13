using BurningKnight.assets;
using BurningKnight.entity.events;
using Lens;
using Lens.entity;
using Lens.graphics;

namespace BurningKnight.ui {
	public class SaveIndicator : Entity {
		private TextureRegion region;
		private bool saving;
		
		public override void Init() {
			base.Init();
			
			AlwaysVisible = true;
			AlwaysActive = true;
			region = CommonAse.Ui.GetSlice("save");

			X = Display.Width - region.Width * 2;
			Y = Display.Height - region.Height * 2;
		}

		public override void Render() {
			// if (saving) {
				Graphics.Render(region, Position);
			// }
		}

		public override bool HandleEvent(Event e) {
			if (e is SaveStartedEvent) {
				saving = true;
			} else if (e is SaveEndedEvent) {
				saving = false;
			}
			
			return base.HandleEvent(e);
		}
	}
}