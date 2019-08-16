using BurningKnight.entity.room.controllable.platform;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.regular {
	public class TwoSidesRoom : RegularRoom {
		private Rect rect;
		private bool vertical;
		
		public override void Paint(Level level) {
			SetupRect();
			Painter.Fill(level, rect, Tile.Chasm);

			var platform = new MovingPlatform();

			platform.X = vertical ? (Random.Int(Left + 1, Right - 1)) * 16 : (Left + GetWidth() / 2) * 16;
			platform.Y = vertical ? (Top + GetHeight() / 2) * 16 : (Random.Int(Top + 1, Bottom - 1)) * 16;
			platform.Direction = new Vector2(vertical ? 0 : 1, vertical ? 1 : 0);

			level.Area.Add(platform);
		}

		public override bool CanConnect(Vector2 P) {
			/*SetupRect();
			
			if (P.X >= rect.Left - 1 && P.X <= rect.Right + 1
			    && P.Y >= rect.Top - 1 && P.Y <= rect.Bottom + 1) {

				return false;
			}*/
			
			return base.CanConnect(P);
		}

		private void SetupRect() {
			if (rect != null) {
				return;
			}
			
			var w = GetWidth() - 2;
			var h = GetHeight() - 2;
			rect = new Rect();
			
			if (w > h || (w == h && Random.Chance())) {
				rect.Top = Top + 1;
				rect.Bottom = Bottom;

				rect.Left = Left + 3 + Random.Int(3);
				rect.Right = Right - 2 - Random.Int(3);
			} else {
				vertical = true;
				rect.Left = Left + 1;
				rect.Right = Right;
				
				rect.Top = Top + 3 + Random.Int(3);
				rect.Bottom = Bottom - 2 - Random.Int(3);
			}
		}
	}
}