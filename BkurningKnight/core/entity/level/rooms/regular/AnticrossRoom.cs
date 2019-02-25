using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class AnticrossRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			byte F = Random.Chance(33) ? Terrain.WALL : (Random.Chance(50) ? Terrain.LAVA : Terrain.CHASM);
			byte Fl = Terrain.RandomFloor();

			if (F == Terrain.LAVA) {
				Fl = Random.Chance(50) ? Terrain.WATER : Terrain.DIRT;
			} 

			int W = Random.NewInt(2, this.GetWidth() / 2 - 4);
			int H = Random.NewInt(2, this.GetHeight() / 2 - 4);
			Painter.Fill(Level, new Rect(this.Left + 2, this.Top + this.GetHeight() / 2 - W, this.Right - 1, this.Top + this.GetHeight() / 2 + W), F);
			Painter.Fill(Level, new Rect(this.Left + this.GetWidth() / 2 - H, this.Top + 2, this.Left + this.GetWidth() / 2 + 2, this.Bottom - 1), F);

			if (Random.Chance(50)) {
				int Side = Random.NewInt(4);
				W -= 1;
				H -= 1;
				Painter.Fill(Level, new Rect(this.Left + 2, this.Top + this.GetHeight() / 2 - W, this.Right - (Side == 1 ? 2 : 3), this.Top + this.GetHeight() / 2 + W), Fl);
				Painter.Fill(Level, new Rect(this.Left + this.GetWidth() / 2 - H, this.Top + (Side == 2 ? 2 : 3), this.Left + this.GetWidth() / 2 + H, this.Bottom - (Side == 3 ? 1 : 2)), Fl);

				if (F != Terrain.LAVA) {
					Fl = Terrain.RandomFloor();
				} 

				W -= 1;
				H -= 1;
				Painter.Fill(Level, new Rect(this.Left + (Side == 0 ? 2 : 3), this.Top + this.GetHeight() / 2 - W, this.Right - (Side == 1 ? 2 : 3), this.Top + this.GetHeight() / 2 + W), Fl);
				Painter.Fill(Level, new Rect(this.Left + this.GetWidth() / 2 - H, this.Top + (Side == 2 ? 2 : 3), this.Left + this.GetWidth() / 2 + H, this.Bottom - (Side == 3 ? 1 : 2)), Fl);
			} 
		}
	}
}
