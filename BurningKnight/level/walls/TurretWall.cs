using BurningKnight.entity.room.controllable.turret;
using BurningKnight.level.rooms;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.walls {
	public class TurretWall : WallPainter {
		public override void Paint(Level level, RoomDef room, Rect inside) {
			base.Paint(level, room, inside);

			var c = room.GetTileCenter() * 16;
			var m = 16;
			var r = Rnd.Chance();
			var t = Rnd.Chance(30);

			if (Rnd.Chance()) {
				if (Rnd.Chance()) {
					level.Area.Add(new QuadRotatingTurret {
						Position = c
					});
				} else {
					level.Area.Add(new QuadTurret {
						Position = c
					});
				}
			} else {
				if (Rnd.Chance()) {
					level.Area.Add(new Turret {
						Position = c + new Vector2(m, m),
						StartingAngle = 1,
						ReverseDirection = t ? !r : r
					});

					level.Area.Add(new Turret {
						Position = c + new Vector2(m, -m),
						StartingAngle = 7,
						ReverseDirection = r
					});

					level.Area.Add(new Turret {
						Position = c + new Vector2(-m, m),
						StartingAngle = 3,
						ReverseDirection = t ? !r : r
					});

					level.Area.Add(new Turret {
						Position = c + new Vector2(-m, -m),
						StartingAngle = 5,
						ReverseDirection = r
					});
				} else {
					level.Area.Add(new Turret {
						Position = c + new Vector2(m, m),
						StartingAngle = 1,
						ReverseDirection = t ? !r : r
					});

					level.Area.Add(new RotatingTurret {
						Position = c + new Vector2(m, -m),
						StartingAngle = 7,
						ReverseDirection = r
					});

					level.Area.Add(new RotatingTurret {
						Position = c + new Vector2(-m, m),
						StartingAngle = 3,
						ReverseDirection = t ? !r : r
					});

					level.Area.Add(new RotatingTurret {
						Position = c + new Vector2(-m, -m),
						StartingAngle = 5,
						ReverseDirection = r
					});
				}
			}
		}
	}
}