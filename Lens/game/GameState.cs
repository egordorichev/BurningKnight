using Lens.entity;

namespace Lens.game {
	public class GameState {
		public Area Area = new Area();
		public Area Ui = new Area();

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

			if (Engine.Version.Debug) {
				Area?.RenderDebug();
			}
		}

		public virtual void RenderUi() {
			Ui.Render();

			if (Engine.Version.Debug) {
				Ui.RenderDebug();
			}
		}
	}
}