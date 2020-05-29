using BurningKnight.entity.room.controllable.platform;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.rooms.regular {
	public class PlatformLineRoom : RegularRoom {
		public override void Paint(Level level) {
			if (Rnd.Chance()) {
				var x = Rnd.Int(Left + 1, Right - 2);
				
				Painter.Fill(level, new Rect().Setup(x, Top + 1, 2, Bottom - 1), Tile.Chasm);
				
				var platform = new MovingPlatform();

				platform.X = x * 16;
				platform.Y = (Rnd.Int(Top + 1, Bottom - 2)) * 16;
				platform.Controller = PlatformController.UpDown;

				level.Area.Add(platform);
			} else {
				var y = Rnd.Int(Top + 1, Bottom - 2);
				
				Painter.Fill(level, new Rect().Setup(Left + 1, y, Right - 1, 2), Tile.Chasm);
				
				var platform = new MovingPlatform();

				platform.X = (Rnd.Int(Left + 1, Right - 2)) * 16;
				platform.Y = y * 16;
				platform.Controller = PlatformController.LeftRight;

				level.Area.Add(platform);
			}
		}
	}
}