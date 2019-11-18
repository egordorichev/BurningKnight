using BurningKnight.entity.creature.npc;
using BurningKnight.level.entities;
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
			
			PaintTunnel(level, Tile.EvilFloor);
			
			var dm = new DarkMage();
			level.Area.Add(dm);

			var w = new Vector2(GetCenterVector().X, (Top + 3) * 16);
			dm.BottomCenter = w;

			for (var i = 0; i < 2; i++) {
				var campfire = new Campfire();
				level.Area.Add(campfire);
				campfire.BottomCenter = w + new Vector2(24 * (i == 0 ? -1 : 1), 0);
			}
		}

		public override bool CanConnect(RoomDef R) {
			return R is BossRoom;
		}
	}
}