using BurningKnight.entity.creature.npc.dungeon;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.special {
	public class VampireRoom : SpecialRoom {
		public override void Paint(Level level) {
			Vampire.Place(GetTileCenter() * 16 + new Vector2(8), level.Area);
		}
	}
}