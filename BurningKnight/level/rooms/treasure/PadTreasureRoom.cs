using System.Collections.Generic;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.treasure {
	public class PadTreasureRoom : TreasureRoom {
		private List<Dot> rects = new List<Dot>();
		private List<Rect> rs = new List<Rect>();
		
		public override void Paint(Level level) {
			Painter.Fill(level, this, 1, Tile.Chasm);

			var m = Random.Int(1, 3);
			var rect = Shrink(m);
			
			var w = Random.Int(3, rect.GetWidth() / 2);
			var h = Random.Int(3, rect.GetHeight() / 2);
			
			PlacePad(level, new Rect(Left + m, Top + m).Resize(w, h));
			PlacePad(level, new Rect(Right - m - w + 1, Top + m).Resize(w, h));
			PlacePad(level, new Rect(Right - m - w + 1, Bottom - m - h + 1).Resize(w, h));
			PlacePad(level, new Rect(Left + m, Bottom - m - h + 1).Resize(w, h));

			var missing = Random.Chance() ? -1 : Random.Int(4);
			var rr = Random.Chance();
			var f = Tiles.RandomFloor();
			
			for (var i = 0; i < rects.Count; i++) {
				if (missing == i) {
					continue;
				}
				
				var r = rects[i];
				var n = rects[(i + 1) % rects.Count];
				
				Painter.DrawLine(level, r, n, rr ? Tiles.RandomFloor() : f);
			}

			foreach (var r in rs) {
				Painter.Fill(level, r, Tile.FloorD);
			}
			
			SetupStands(level);
		}

		private void PlacePad(Level level, Rect rect) {
			var c = new Dot(rect.Left + rect.GetWidth() / 2, rect.Top + rect.GetHeight() / 2);
			PlaceStand(level, c * 16);
			rects.Add(c);
			rs.Add(rect);
		}

		public override int GetMinWidth() {
			return 12;
		}

		public override int GetMinHeight() {
			return 12;
		}
	}
}