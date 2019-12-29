using BurningKnight.entity.creature.npc.dungeon;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.special.shop {
	public class TrashGoblinRoom : NpcShopRoom {
		public override void Paint(Level level) {
			TrashGoblin.Place(GetTileCenter() * 16 + new Vector2(8, 8), level.Area);
		}
	}
}