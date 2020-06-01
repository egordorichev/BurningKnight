using BurningKnight.entity.creature.player;
using BurningKnight.save;
using BurningKnight.state;

namespace BurningKnight.entity.twitch.happening {
	public class FloorResetHappening : Happening {
		public override void Happen(Player player) {
			SaveManager.Delete(SaveType.Level);
			InGameState.IgnoreSave = true;

			Run.Redo = true;
		}
	}
}