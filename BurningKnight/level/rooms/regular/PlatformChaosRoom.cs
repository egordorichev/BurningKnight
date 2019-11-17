using BurningKnight.entity.room.controllable.platform;
using BurningKnight.entity.room.controllable.turret;
using BurningKnight.level.tile;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.regular {
	public class PlatformChaosRoom : RegularRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, 3, Tile.Chasm);

			var c = Rnd.Int(2, 4);
			var p = Rnd.Chance(30);
			
			for (var i = 0; i < c; i++) {
				var platform = new MovingPlatform();

				platform.X = Rnd.Int(Left + 4, Right - 4) * 16;
				platform.Y = Rnd.Int(Top + 4, Bottom - 4) * 16;
				platform.Controller = Rnd.Chance() ? PlatformController.ClockWise : PlatformController.CounterClockWise;

				level.Area.Add(platform);
				
				if (p && Rnd.Chance()) {
					var turret = new RotatingTurret();
					level.Area.Add(turret);
					turret.Center = platform.Position + new Vector2(16, 12);
				}
			}
		}
		
		public override int GetMinWidth() {
			return 12;
		}

		public override int GetMinHeight() {
			return 10;
		}

		public override int GetMaxWidth() {
			return 20;
		}

		public override int GetMaxHeight() {
			return 18;
		}
	}
}