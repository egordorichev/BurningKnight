using BurningKnight.entity.creature.npc.dungeon;

namespace BurningKnight.level.rooms.special.npc {
	public class ElonRoom : SpecialRoom {
		public override void Paint(Level level) {
			base.Paint(level);
			DungeonElon.Place(GetCenter() * 16, level.Area);
		}

		public override int GetMinWidth() {
			return 6;
		}

		public override int GetMaxWidth() {
			return 11;
		}

		public override int GetMinHeight() {
			return 6;
		}

		public override int GetMaxHeight() {
			return 11;
		}

		protected override bool Quad() {
			return true;
		}
	}
}