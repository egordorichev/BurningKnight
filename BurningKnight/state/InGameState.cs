using BurningKnight.entity;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.ui;
using BurningKnight.physics;
using BurningKnight.save;
using BurningKnight.util;
using Lens;
using Lens.entity;
using Lens.game;
using Lens.util.camera;
using Console = BurningKnight.debug.Console;

namespace BurningKnight.state {
	public class InGameState : GameState {		
		public InGameState(Area area) {
			Area = area;
		}
		
		public override void Init() {
			base.Init();
			SetupUi();
		}

		public override void Destroy() {
			Physics.Destroy();
			// Fixme: enable
			// SaveManager.SaveAll(Area);
			Area = null;

			// Clears the area, but we don't want that, cause we are still saving
			base.Destroy();
		}

		public override void Update(float dt) {
			Physics.Update(dt);
			base.Update(dt);
		}

		public override void Render() {
			base.Render();
			Physics.Render();
		}

		private void SetupUi() {
			Ui.Add(new Camera(new FollowingDriver()));
			
			var cursor = new Cursor();
			Ui.Add(cursor);

			var player = LocalPlayer.Locate(Area);
			
			Camera.Instance.Follow(player, 1f);
			Camera.Instance.Follow(cursor, 1f);
			Camera.Instance.Jump();

			if (Engine.Version.Debug) {
				Ui.Add(new Console(Area));
			}
			
			Ui.Add(new UiInventory(player));
		}
	}
}