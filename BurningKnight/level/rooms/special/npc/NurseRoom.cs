using BurningKnight.entity.creature.npc.dungeon;

namespace BurningKnight.level.rooms.special.npc {
	public class NurseRoom : SpecialRoom {
		public override void Paint(Level level) {
			Nurse.Place(GetCenter() * 16, level.Area);
		}

		public override int GetMinWidth() {
			return 6;
		}

		public override int GetMaxWidth() {
			return 7;
		}

		public override int GetMinHeight() {
			return 6;
		}

		public override int GetMaxHeight() {
			return 7;
		}
	}
}