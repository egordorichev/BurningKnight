using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.treasure {
	public class BrokeLineTreasureRoom : TreasureRoom {
		public override void Paint(Level Level) {
			base.Paint(Level);
			var F = Random.Chance(50) ? Terrain.FLOOR_B : Terrain.FLOOR_D;
			var Fl = Random.Chance(50) ? Terrain.WALL : Terrain.CHASM;
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.FLOOR_D);
			Painter.Fill(Level, this, 2, Fl);
			var El = Random.Chance(50);

			if (El)
				Painter.FillEllipse(Level, this, 3, F);
			else
				Painter.Fill(Level, this, 3, F);


			var S = false;
			var All = Random.Chance(50);

			if (All || Random.Chance(50)) {
				Painter.Set(Level, new Point(GetWidth() / 2 + Left, Top + 2), F);
				S = true;
			}

			if (All || Random.Chance(50)) {
				Painter.Set(Level, new Point(GetWidth() / 2 + Left, Bottom - 2), F);
				S = true;
			}

			if (All || Random.Chance(50)) {
				Painter.Set(Level, new Point(Left + 2, GetHeight() / 2 + Top), F);
				S = true;
			}

			if (All || Random.Chance(50) || !S) Painter.Set(Level, new Point(Right - 2, GetHeight() / 2 + Top), F);

			if (Random.Chance(50)) {
				var Flr = Terrain.RandomFloor();

				if (Random.Chance(50)) {
					Painter.Fill(Level, new Rect(Left + 1, Top + 1, Left + 4, Top + 4), Flr);
					Painter.Fill(Level, new Rect(Right - 3, Top + 1, Right, Top + 4), Flr);
					Painter.Fill(Level, new Rect(Left + 1, Bottom - 3, Left + 4, Bottom), Flr);
					Painter.Fill(Level, new Rect(Right - 3, Bottom - 3, Right, Bottom), Flr);
				}
				else {
					Painter.FillEllipse(Level, new Rect(Left + 1, Top + 1, Left + 4, Top + 4), Flr);
					Painter.FillEllipse(Level, new Rect(Right - 3, Top + 1, Right, Top + 4), Flr);
					Painter.FillEllipse(Level, new Rect(Left + 1, Bottom - 3, Left + 4, Bottom), Flr);
					Painter.FillEllipse(Level, new Rect(Right - 3, Bottom - 3, Right, Bottom), Flr);
				}
			}

			PlaceChest(GetCenter());
		}
	}
}