using BurningKnight.entity.room.controllable.platform;
using BurningKnight.level.tile;
using Lens.util.math;

namespace BurningKnight.level.rooms.regular {
	public class PlatformChaosRoom : RegularRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, 3, Tile.Chasm);

			var c = Random.Int(2, 4);
			
			for (var i = 0; i < c; i++) {
				var platform = new MovingPlatform();

				platform.X = Random.Int(Left + 4, Right - 4) * 16;
				platform.Y = Random.Int(Top + 4, Bottom - 4) * 16;
				platform.Controller = Random.Chance() ? PlatformController.ClockWise : PlatformController.CounterClockWise;

				level.Area.Add(platform);
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