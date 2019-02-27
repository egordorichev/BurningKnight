using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.item {
	public class BrokeLineItemRoomDef : ItemRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);
			var F = Terrain.RandomFloor();
			var Fl = Random.Chance(50) ? Terrain.WALL : Terrain.CHASM;
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.RandomFloor());
			Painter.Fill(Level, this, 3, Fl);
			var El = Random.Chance(50);

			if (El)
				Painter.FillEllipse(Level, this, 4, F);
			else
				Painter.Fill(Level, this, 4, F);


			var S = false;
			var All = true;

			if (All || Random.Chance(50)) {
				Painter.Set(Level, new Point(GetWidth() / 2 + Left, Top + 3), F);
				S = true;
			}

			if (All || Random.Chance(50)) {
				Painter.Set(Level, new Point(GetWidth() / 2 + Left, Bottom - 3), F);
				S = true;
			}

			if (All || Random.Chance(50)) {
				Painter.Set(Level, new Point(Left + 3, GetHeight() / 2 + Top), F);
				S = true;
			}

			if (All || Random.Chance(50) || !S) Painter.Set(Level, new Point(Right - 3, GetHeight() / 2 + Top), F);

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

			PlaceItem(GetCenter());
		}
	}
}