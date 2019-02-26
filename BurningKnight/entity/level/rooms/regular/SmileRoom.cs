using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class SmileRoom : RegularRoom {
		public override void Paint(Level Level) {
			base.Paint(Level);
			var Fill = Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA;
			var F = Terrain.RandomFloor();
			Painter.FillEllipse(Level, this, 2, Fill);
			Painter.FillEllipse(Level, this, 3, F);
			float W = GetWidth();
			float H = GetHeight();
			var R = new Rect(Left + 1, (int) Math.Floor(Top + H / 2), (int) (Left + W - 1), Math.Floor(H / 2) + Math.Ceil(Top + H / 2) - 1);
			Painter.Fill(Level, R, Fill == Terrain.LAVA ? Terrain.DIRT : F);
			Painter.Fill(Level, R, 1, Fill);
			Painter.Fill(Level, new Rect(Left + (int) Math.Floor(W / 2) - 1, Top + (int) Math.Floor(H / 2) + 1, Left + (int) Math.Ceil(W / 2) + 1, Bottom - 1), Fill == Terrain.LAVA ? Terrain.DIRT : F);
		}

		public override int GetMinHeight() {
			return 7;
		}

		public override int GetMinWidth() {
			return 9;
		}
	}
}