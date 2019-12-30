using BurningKnight.level.entities.statue;

namespace BurningKnight.level.rooms.special.statue {
	public class ChestStatueRoom : StatueRoom {
		protected override Statue GetStatue() {
			return new ChestStatue();
		}
	}
}