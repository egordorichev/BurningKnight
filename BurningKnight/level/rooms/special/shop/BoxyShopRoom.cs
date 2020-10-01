using BurningKnight.entity.creature.npc.dungeon;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.special.shop {
	public class BoxyShopRoom : NpcShopRoom {
		public override void Paint(Level level) {
			base.Paint(level);
			Boxy.Place(GetTileCenter() * 16 + new Vector2(8, 8), level.Area);
		}
	}
}