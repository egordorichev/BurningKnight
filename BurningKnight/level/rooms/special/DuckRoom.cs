using BurningKnight.entity.creature.npc.dungeon;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.special {
	public class DuckRoom : SpecialRoom {
		public override void Paint(Level level) {
			DungeonDuck.Place(GetTileCenter() * 16 + new Vector2(8, 8), level.Area);
		}

		public override int GetMinWidth() {
			return 7;
		}

		public override int GetMaxWidth() {
			return 12;
		}

		public override int GetMinHeight() {
			return 7;
		}

		public override int GetMaxHeight() {
			return 12;
		}
	}
}