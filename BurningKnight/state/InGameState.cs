using BurningKnight.physics;
using BurningKnight.save;
using Lens.entity;
using Lens.game;
using Lens.util.camera;

namespace BurningKnight.state {
	public class InGameState : GameState {
		public InGameState(Area area) {
			Area = area;
		}
		
		public override void Init() {
			base.Init();
			
			Area.Add(new Camera());
		}

		public override void Destroy() {
			Physics.Destroy();
			SaveManager.SaveAll(Area);
			Area = null;

			// Clears the area, but we don't want that, cause we are still saving
			base.Destroy();
		}

		public override void Update(float dt) {
			base.Update(dt);
			Physics.Update(dt);
		}

		public override void Render() {
			base.Render();
			Physics.Render();
		}
	}
}