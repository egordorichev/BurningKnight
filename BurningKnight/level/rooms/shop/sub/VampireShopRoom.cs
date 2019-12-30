using BurningKnight.entity.creature.npc.dungeon;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.shop.sub {
	public class VampireShopRoom : SubShopRoom {
		public override void Paint(Level level) {
			Vampire.Place(GetTileCenter() * 16 + new Vector2(8), level.Area);
		}
	}
}