using Lens.Entity;
using Microsoft.Xna.Framework;

namespace Lens.State {
	public class GameState {
		protected Area area;

		public void Init() {
			area = new Area();
		}

		public void Destroy() {
			area.Destroy();
		}

		public void Update(float dt) {
			area.Update(dt);
		}

		public void Render() {
			Engine.Graphics.GraphicsDevice.Clear(Color.Black);
			area.Render();

			if (Engine.Version.Debug) {
				area.RenderDebug();
			}
		}
	}
}