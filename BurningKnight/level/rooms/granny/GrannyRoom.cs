using BurningKnight.entity.creature.npc;
using BurningKnight.level.rooms.boss;
using BurningKnight.level.rooms.special;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.granny {
	public class GrannyRoom : SpecialRoom {
		public override void Paint(Level level) {
			var granny = new Granny();
			level.Area.Add(granny);
			granny.BottomCenter = new Vector2(Left + GetWidth() / 2 + 0.5f, Top + 2) * 16;
		}

		public override bool CanConnect(RoomDef R) {
			return R is BossRoom;
		}
	}
}