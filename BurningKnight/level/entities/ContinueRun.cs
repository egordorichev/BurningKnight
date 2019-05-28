using BurningKnight.save;
using BurningKnight.state;
using Lens;
using Lens.entity;

namespace BurningKnight.level.entities {
	public class ContinueRun : Exit {
		public override void Init() {
			base.Init();

			if (Engine.EditingLevel) {
				return;
			}

			if (SaveManager.ExistsAndValid(SaveType.Game)
				&& SaveManager.ExistsAndValid(SaveType.Level, $"{SaveManager.SlotDir}level-1.lvl")
				&& SaveManager.ExistsAndValid(SaveType.Player)) {

				return;
			}
			
			Done = true;
		}

		protected override bool Interact(Entity entity) {
			Run.Depth = 1;
			return true;
		}

		protected override string GetFxText() {
			return "continue_run";
		}
	}
}