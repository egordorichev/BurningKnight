using Lens.entity;
using Microsoft.Xna.Framework;

namespace Lens.game {
	public class GameState {
		protected Area area;

		public virtual void Init() {
			area = new Area();
		}

		public virtual void Destroy() {
			area.Destroy();
		}

		public virtual void Update(float dt) {
			area.Update(dt);
		}

		public virtual void Render() {
			area.Render();

			if (Engine.Version.Debug) {
				area.RenderDebug();
			}
		}
	}
}