using BurningKnight.level.entities.statue;

namespace BurningKnight.level.rooms.special.statue {
	public class StoneStatueRoom : StatueRoom {
		protected override Statue GetStatue() {
			return new StoneStatue();
		}
	}
}