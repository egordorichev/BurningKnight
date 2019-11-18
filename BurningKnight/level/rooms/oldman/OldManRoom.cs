using BurningKnight.entity.creature.npc;
using BurningKnight.level.rooms.boss;
using BurningKnight.level.rooms.special;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.oldman {
	public class OldManRoom : SpecialRoom {
		public override void Paint(Level level) {
			var dm = new DarkMage();
			level.Area.Add(dm);
			dm.BottomCenter = new Vector2(Left + GetWidth() / 2 + 0.5f, Top + 2) * 16;
		}

		public override bool CanConnect(RoomDef R) {
			return R is BossRoom;
		}
	}
}