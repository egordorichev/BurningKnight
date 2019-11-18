using BurningKnight.entity.room.controllable.platform;
using BurningKnight.entity.room.controllable.turret;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.regular {
	public class TwoSidesRoom : RegularRoom {
		private Rect rect;
		private bool vertical;

		public TwoSidesRoom() {
			vertical = Rnd.Chance();
		}

		public override void Paint(Level level) {
			SetupRect();
			Painter.Fill(level, rect, Tile.Chasm);

			var platform = new MovingPlatform();

			platform.X = vertical ? (Rnd.Int(Left + 2, Right - 2)) * 16 : (Left + GetWidth() / 2) * 16;
			platform.Y = vertical ? (Top + GetHeight() / 2) * 16 : (Rnd.Int(Top + 2, Bottom - 2)) * 16;
			platform.Controller = Rnd.Chance(40) ? (Rnd.Chance() ? PlatformController.ClockWise : PlatformController.CounterClockWise) : (vertical ? PlatformController.UpDown : PlatformController.LeftRight);

			level.Area.Add(platform);

			if (Rnd.Chance(30)) {
				var turret = new RotatingTurret();
				level.Area.Add(turret);
				turret.Center = platform.Position + new Vector2(16, 12);
			}
		}

		public override bool CanConnect(RoomDef R, Dot P) {
			var x = (int) P.X;
			var y = (int) P.Y;
			
			if (vertical) {
				if ((x == Left || x == Right) && (y > Top + 3 && y < Bottom - 4)) {
					return false;
				}	
			} else {
				if ((y == Top || y == Bottom) && (x > Left + 3 && x < Right - 4)) {
					return false;
				}	
			}
			
			return base.CanConnect(R, P);
		}

		private void SetupRect() {
			if (rect != null) {
				return;
			}
			
			rect = new Rect();
			
			if (!vertical) {
				rect.Top = Top + 1;
				rect.Bottom = Bottom;

				rect.Left = Left + 3 + Rnd.Int(3);
				rect.Right = Right - 2 - Rnd.Int(3);
			} else {
				rect.Left = Left + 1;
				rect.Right = Right;
				
				rect.Top = Top + 3 + Rnd.Int(3);
				rect.Bottom = Bottom - 2 - Rnd.Int(3);
			}
		}
		
		public override int GetMinWidth() {
			return vertical ? 8 : 13;
		}

		public override int GetMinHeight() {
			return vertical ? 13 : 8;
		}

		public override int GetMaxWidth() {
			return vertical ? 12 : 20;
		}

		public override int GetMaxHeight() {
			return vertical ? 12 : 20;
		}
	}
}