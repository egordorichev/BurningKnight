using BurningKnight.level.entities.statue;

namespace BurningKnight.level.rooms.special.statue {
	public class WellRoom : StatueRoom {
		protected override Statue GetStatue() {
			return new Well();
		}
	}
}