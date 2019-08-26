using BurningKnight.level.tile;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.connection {
	public class IntersectionConnectionRoom : ConnectionRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tile.Chasm);

			var b = Random.Chance();
			var a = Random.Chance();

			foreach (var d in Connected.Values) {
				Vector2 to;
				Vector2 from;

				if (d.X == Left) {
					to = new Vector2(Right - 1, d.Y);
					from = new Vector2(d.X + 1, d.Y);
				} else if (d.X == Right) {
					to = new Vector2(Left + 1, d.Y);
					from = new Vector2(d.X - 1, d.Y);
				} else if (d.Y == Top) {
					to = new Vector2(d.X, Bottom - 1);
					from = new Vector2(d.X, d.Y + 1);
				} else {
					to = new Vector2(d.X, Top + 1);
					from = new Vector2(d.X, d.Y - 1);
				}
				
				Painter.DrawLine(level, from, to, Tiles.RandomFloor(), b && (a || Random.Chance()));
			}
		}

		public override int GetMinWidth() {
			return 5;
		}

		public override int GetMinHeight() {
			return 5;
		}
	}
}