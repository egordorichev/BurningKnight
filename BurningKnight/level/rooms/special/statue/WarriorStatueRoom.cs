using BurningKnight.level.entities.statue;

namespace BurningKnight.level.rooms.special.statue {
	public class WarriorStatueRoom : StatueRoom {
		protected override Statue GetStatue() {
			return new WarriorStatue();		
		}
	}
}