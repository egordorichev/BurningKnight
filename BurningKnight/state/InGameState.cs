using BurningKnight.entity;
using BurningKnight.entity.creature.player;
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
			
			Ui.Add(new Camera());
			
			var cursor = new Cursor();
			Ui.Add(cursor);
			
			Camera.Instance.Follow(LocalPlayer.Locate(Area), 1f);
			Camera.Instance.Follow(cursor, 0.5f);
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