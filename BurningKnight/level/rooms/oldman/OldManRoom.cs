using BurningKnight.entity.creature.npc;
using BurningKnight.level.rooms.boss;
using BurningKnight.level.rooms.special;
using BurningKnight.level.tile;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.oldman {
	public class OldManRoom : SpecialRoom {
		public override void PaintFloor(Level level) {
			
		}

		public override void Paint(Level level) {
			Painter.Rect(level, this, 1, Tile.EvilWall);
			Painter.Fill(level, this, 2, Tile.EvilFloor);
			
			var dm = new DarkMage();
			level.Area.Add(dm);
			dm.BottomCenter = new Vector2(Left + GetWidth() / 2 + 0.5f, Top + 2) * 16;
		}

		public override bool CanConnect(RoomDef R) {
			return R is BossRoom;
		}
	}
}