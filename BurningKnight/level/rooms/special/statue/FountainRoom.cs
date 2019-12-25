using BurningKnight.level.entities.statue;

namespace BurningKnight.level.rooms.special.statue {
	public class FountainRoom : StatueRoom {
		protected override Statue GetStatue() {
			return new Fountain();		
		}
	}
}