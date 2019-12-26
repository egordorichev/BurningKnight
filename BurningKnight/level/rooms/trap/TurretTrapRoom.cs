using BurningKnight.entity.room.controllable.turret;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.trap {
	public class TurretTrapRoom : TrapRoom {
		public override void PaintFloor(Level level) {
			
		}

		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tile.Chasm);
			Painter.Fill(level, this, 2, Tiles.RandomFloor());

			var s = 3;
			
			for (var x = Left + 2; x < Right - 2; x += s) {
				Place(level, x, Top + 1, 2);	
			}
			
			for (var x = Left + 2; x < Right - 2; x += s) {
				Place(level, x, Bottom - 1, 6);	
			}
			
			for (var y = Top + 2; y < Bottom - 2; y += s) {
				Place(level, Left + 1, y, 0);	
			}
			
			for (var y = Top + 2; y < Bottom - 2; y += s) {
				Place(level, Right - 1, y, 4);	
			}
		}

		public override bool CanConnect(RoomDef R, Dot P) {
			if (P.X == Left || P.X == Right) {
				if (P.Y == Top + 1 || P.Y == Bottom - 1) {
					return false;
				}
			} else 	if (P.X == Left + 1 || P.X == Right - 1) {
				return false;
			}

			return base.CanConnect(R, P);
		}

		private void Place(Level level, int x, int y, uint a) {
			foreach (var d in Connected.Values) {
				if ((d.X == x && (d.Y == y + 1 || d.Y == y - 1)) || (d.Y == y && (d.X == x + 1 || d.X == x - 1))) {
					return;
				}
			}
			
			Painter.Set(level, x, y, Tile.FloorA);

			level.Area.Add(new Turret {
				Position = new Vector2(x, y) * 16,
				StartingAngle = a
			});
		}
	}
}