using BurningKnight.entity.creature.player;
using BurningKnight.save;
using BurningKnight.state;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.util;

namespace BurningKnight.level.entities {
	public class ContinueRun : Exit {
		public ContinueRun() {
			if (Engine.EditingLevel) {
				return;
			}

			var a = SaveManager.ExistsAndValid(SaveType.Game);
			var b = SaveManager.ExistsAndValid(SaveType.Level, null, $"{SaveManager.SlotDir}level-1.lvl");
			var c = SaveManager.ExistsAndValid(SaveType.Player);

			if (a && b && c) {
				return;
			}
			
			Done = true;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Run.LastSavedDepth == 0) {
				Done = true;
			}
		}

		protected override bool Interact(Entity entity) {
			Log.Error($"Go to runs last saved depth to {Run.LastSavedDepth}");
			Run.Depth = Run.LastSavedDepth;
			entity.RemoveComponent<PlayerInputComponent>();
			
			return true;
		}

		protected override string GetFxText() {
			return Locale.Get("continue_run");
		}
	}
}