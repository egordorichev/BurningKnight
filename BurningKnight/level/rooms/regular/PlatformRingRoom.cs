using System;
using BurningKnight.entity.room.controllable.platform;
using BurningKnight.level.tile;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.regular {
	public class PlatformRingRoom : RegularRoom {
		public override void Paint(Level level) {
			Painter.Rect(level, this, 3, Tile.Chasm);
			Painter.Rect(level, this, 4, Tile.Chasm);

			var c = Rnd.Int(1, 5);
			var d = Rnd.Chance() ? PlatformController.ClockWise : PlatformController.CounterClockWise;
			
			for (var i = 0; i < c; i++) {
				var platform = new MovingPlatform();
				platform.StartingStep = i;

				if (i == 0) {
					platform.Position = new Vector2(Left + 3, Top + 3) * 16;
				} else if (i == 2 || (i == 1 && c == 2)) { // For symetry
					platform.Position = new Vector2(Right - 4, Bottom - 4) * 16;
					platform.StartingStep = 2;
				} else if (i == 1) {
					platform.Position = new Vector2(Right - 4, Top + 3) * 16;
				} else if (i == 3) {
					platform.Position = new Vector2(Left + 3, Bottom - 4) * 16;
				}
				
				platform.Controller = d;

				level.Area.Add(platform);
			}
		}

		public override int GetMinHeight() {
			return 12;
		}

		public override int GetMinWidth() {
			return 12;
		}
	}
}