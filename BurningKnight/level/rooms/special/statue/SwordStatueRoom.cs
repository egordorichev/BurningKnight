using BurningKnight.level.entities.statue;

namespace BurningKnight.level.rooms.special.statue {
	public class SwordStatueRoom : StatueRoom {
		protected override Statue GetStatue() {
			return new SwordStatue();
		}
	}
}