using BurningKnight.level.entities.statue;

namespace BurningKnight.level.rooms.special.statue {
	public class ScourgeStatueRoom : StatueRoom {
		protected override Statue GetStatue() {
			return new ScourgeStatue();
		}
	}
}