using BurningKnight.entity.creature.npc.dungeon;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.special.shop {
	public class GobettaShopRoom : NpcShopRoom {
		public override void Paint(Level level) {
			base.Paint(level);
			Gobetta.Place(GetTileCenter() * 16 + new Vector2(8, 8), level.Area);
		}
	}
}