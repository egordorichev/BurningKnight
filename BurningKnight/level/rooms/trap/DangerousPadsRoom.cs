using System;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.rooms.trap {
	public class DangerousPadsRoom : TrapRoom {
		public override void PaintFloor(Level level) {
			
		}

		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tile.Chasm);

			var gap = 2;
			
			var xcollumn = Rnd.Int(2, 5);
			var ycollumn = Rnd.Chance(20) ? Rnd.Int(2, 5) : xcollumn;
			var w = GetWidth();
			var h = GetHeight();
			var gx = (gap + xcollumn);
			var gy = (gap + ycollumn);
			var mx = Rnd.Chance(50);
			var my = !mx && Rnd.Chance(50);
			var mxd = Rnd.Chance() ? -1 : 1;
			var myd = Rnd.Chance() ? -1 : 1;
			var mxs = Rnd.Int(2);
			var mys = Rnd.Int(2);

			var xcount = (int) Math.Floor((w - (float) xcollumn) / gx);
			var ycount = (int) Math.Floor((h - (float) ycollumn) / gy);

			var xw = xcount * gx - xcollumn;
			var yw = ycount * gy - ycollumn;

			var xo = (int) Math.Floor((w - xw) / 2f);
			var yo = (int) Math.Floor((h - yw) / 2f);
			
			// Painter.Fill(level, new Rect(Left + xo, Top + yo).Resize(xw, yw), Tile.FloorA);

			for (var x = 0; x < xcount; x++) {
				for (var y = 0; y < ycount; y++) {
					Painter.Fill(level, new Rect(
						Left + xo + x * gx + (mx && y % 2 == mxs ? mxd : 0), 
						Top + yo + y * gy + (my && x % 2 == mys ? myd : 0)
					).Resize(xcollumn, ycollumn), Rnd.Chance(5) ? Tiles.RandomFloor() : Tile.SensingSpikeTmp);
				}
			}

			foreach (var d in Connected.Values) {
				Painter.Fill(level, new Rect(d.X - 2, d.Y - 2, d.X + 3, d.Y + 3), Tiles.RandomFloor());
			}
		}
		
		public override int GetMinWidth() {
			return 20;
		}

		public override int GetMinHeight() {
			return 20;
		}

		public override int GetMaxWidth() {
			return 28;
		}

		public override int GetMaxHeight() {
			return 28;
		}
	}
}