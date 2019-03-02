using Lens.entity;

namespace Lens.game {
	public class GameState {
		public Area Area = new Area();
		public Area Ui = new Area();

		public static bool RenderDebug = false;
		
		public virtual void Init() {
			
		}

		public virtual void Destroy() {
			Area?.Destroy();
			Ui.Destroy();
		}

		public virtual void Update(float dt) {
			Area?.Update(dt);
			Ui.Update(dt);
		}

		public virtual void Render() {
			Area?.Render();

			if (RenderDebug) {
				Area?.RenderDebug();
			}
		}

		public virtual void RenderUi() {
			Ui.Render();

			if (RenderDebug) {
				Ui.RenderDebug();
			}
		}
	}
}