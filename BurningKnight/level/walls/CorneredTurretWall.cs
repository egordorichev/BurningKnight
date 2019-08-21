using BurningKnight.entity.room.controllable.turret;
using BurningKnight.level.rooms;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.walls {
	public class CorneredTurretWall : WallPainter {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			base.Paint(level, room, inside);

			level.Area.Add(new Turret {
				Position = new Vector2(room.Left + 2, room.Top + 2) * 16,
				StartingAngle = 1
			});
			
			level.Area.Add(new Turret {
				Position = new Vector2(room.Right - 2, room.Top + 2) * 16,
				StartingAngle = 3
			});
			
			level.Area.Add(new Turret {
				Position = new Vector2(room.Right - 2, room.Bottom - 2) * 16,
				StartingAngle = 5
			});

			level.Area.Add(new Turret {
				Position = new Vector2(room.Left + 2, room.Bottom - 2) * 16,
				StartingAngle = 7
			});
		}
	}
}