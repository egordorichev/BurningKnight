using BurningKnight.entity.creature.npc;
using BurningKnight.level.rooms.boss;
using BurningKnight.level.rooms.special;
using BurningKnight.level.tile;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.granny {
	public class GrannyRoom : SpecialRoom {
		public override void PaintFloor(Level level) {
			
		}

		public override void Paint(Level level) {
			Painter.Rect(level, this, 1, Tile.GrannyWall);
			Painter.Fill(level, this, 2, Tile.GrannyFloor);
			
			var granny = new Granny();
			level.Area.Add(granny);
			granny.BottomCenter = new Vector2(Left + GetWidth() / 2 + 0.5f, Top + 3) * 16;
		}

		public override bool CanConnect(RoomDef R) {
			return R is BossRoom;
		}
	}
}