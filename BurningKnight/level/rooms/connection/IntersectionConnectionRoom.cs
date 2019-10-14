using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.connection {
	public class IntersectionConnectionRoom : ConnectionRoom {
		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tiles.Pick(Tile.Chasm, Tile.Chasm, Tile.Lava, Tile.WallA, Tile.Planks));

			var b = Random.Chance();
			var a = Random.Chance();

			var left = false;
			var right = false;
			var top = false;
			var bottom = false;

			foreach (var d in Connected.Values) {
				Dot to;
				Dot from;

				if (d.X == Left) {
					left = true;
					to = new Dot(Right - 1, d.Y);
					from = new Dot(d.X + 1, d.Y);
				} else if (d.X == Right) {
					right = true;
					to = new Dot(Left + 1, d.Y);
					from = new Dot(d.X - 1, d.Y);
				} else if (d.Y == Top) {
					top = true;
					to = new Dot(d.X, Bottom - 1);
					from = new Dot(d.X, d.Y + 1);
				} else {
					bottom = true;
					to = new Dot(d.X, Top + 1);
					from = new Dot(d.X, d.Y - 1);
				}
				
				Painter.DrawLine(level, from, to, Tiles.RandomFloor(), b && (a || Random.Chance()));
			}

			if ((right || left) && !(bottom || top)) {
				var x = Random.Int(Left + 1, Right - 1);
				Painter.DrawLine(level, new Dot(x, Top + 1), new Dot(x, Bottom - 1), Tile.FloorD, b && (a || Random.Chance()));
			} else if ((top || bottom) && !(left || right)) {
				var y = Random.Int(Top + 1, Bottom - 1);
				Painter.DrawLine(level, new Dot(Left + 1, y), new Dot(Right - 1, y), Tile.FloorD, b && (a || Random.Chance()));
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