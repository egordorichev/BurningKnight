using BurningKnight.entity.creature.npc.dungeon;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.special {
	public class DuckRoom : SpecialRoom {
		public override void Paint(Level level) {
			DungeonDuck.Place(GetCenter() * 16 + new Vector2(8, 8), level.Area);
		}

		public override int GetMinWidth() {
			return 6;
		}

		public override int GetMaxWidth() {
			return 7;
		}

		public override int GetMinHeight() {
			return 5;
		}

		public override int GetMaxHeight() {
			return 6;
		}
	}
}