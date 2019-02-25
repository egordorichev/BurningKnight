using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class SmileRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			byte Fill = Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA;
			byte F = Terrain.RandomFloor();
			Painter.FillEllipse(Level, this, 2, Fill);
			Painter.FillEllipse(Level, this, 3, F);
			float W = this.GetWidth();
			float H = this.GetHeight();
			Rect R = new Rect(this.Left + 1, (int) Math.Floor(this.Top + H / 2), (int) (this.Left + W - 1), (int) (Math.Floor(H / 2) + Math.Ceil(this.Top + H / 2) - 1));
			Painter.Fill(Level, R, Fill == Terrain.LAVA ? Terrain.DIRT : F);
			Painter.Fill(Level, R, 1, Fill);
			Painter.Fill(Level, new Rect(this.Left + (int) Math.Floor(W / 2) - 1, this.Top + (int) Math.Floor(H / 2) + 1, this.Left + (int) Math.Ceil(W / 2) + 1, this.Bottom - 1), Fill == Terrain.LAVA ? Terrain.DIRT : F);
		}

		public override int GetMinHeight() {
			return 7;
		}

		public override int GetMinWidth() {
			return 9;
		}
	}
}
