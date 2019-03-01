using Lens.entity;

namespace Lens.game {
	public class GameState {
		public Area Area = new Area();

		public virtual void Init() {
			
		}

		public virtual void Destroy() {
			Area?.Destroy();
		}

		public virtual void Update(float dt) {
			Area?.Update(dt);
		}

		public virtual void Render() {
			Area?.Render();

			if (Engine.Version.Debug) {
				Area?.RenderDebug();
			}
		}
	}
}