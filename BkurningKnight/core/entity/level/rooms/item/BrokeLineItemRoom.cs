using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.item {
	public class BrokeLineItemRoom : ItemRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			byte F = Terrain.RandomFloor();
			byte Fl = Random.Chance(50) ? Terrain.WALL : Terrain.CHASM;
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.RandomFloor());
			Painter.Fill(Level, this, 3, Fl);
			bool El = Random.Chance(50);

			if (El) {
				Painter.FillEllipse(Level, this, 4, F);
			} else {
				Painter.Fill(Level, this, 4, F);
			}


			bool S = false;
			bool All = true;

			if (All || Random.Chance(50)) {
				Painter.Set(Level, new Point(this.GetWidth() / 2 + this.Left, this.Top + 3), F);
				S = true;
			} 

			if (All || Random.Chance(50)) {
				Painter.Set(Level, new Point(this.GetWidth() / 2 + this.Left, this.Bottom - 3), F);
				S = true;
			} 

			if (All || Random.Chance(50)) {
				Painter.Set(Level, new Point(this.Left + 3, this.GetHeight() / 2 + this.Top), F);
				S = true;
			} 

			if (All || Random.Chance(50) || !S) {
				Painter.Set(Level, new Point(this.Right - 3, this.GetHeight() / 2 + this.Top), F);
			} 

			if (Random.Chance(50)) {
				byte Flr = Terrain.RandomFloor();

				if (Random.Chance(50)) {
					Painter.Fill(Level, new Rect(this.Left + 1, this.Top + 1, this.Left + 4, this.Top + 4), Flr);
					Painter.Fill(Level, new Rect(this.Right - 3, this.Top + 1, this.Right, this.Top + 4), Flr);
					Painter.Fill(Level, new Rect(this.Left + 1, this.Bottom - 3, this.Left + 4, this.Bottom), Flr);
					Painter.Fill(Level, new Rect(this.Right - 3, this.Bottom - 3, this.Right, this.Bottom), Flr);
				} else {
					Painter.FillEllipse(Level, new Rect(this.Left + 1, this.Top + 1, this.Left + 4, this.Top + 4), Flr);
					Painter.FillEllipse(Level, new Rect(this.Right - 3, this.Top + 1, this.Right, this.Top + 4), Flr);
					Painter.FillEllipse(Level, new Rect(this.Left + 1, this.Bottom - 3, this.Left + 4, this.Bottom), Flr);
					Painter.FillEllipse(Level, new Rect(this.Right - 3, this.Bottom - 3, this.Right, this.Bottom), Flr);
				}

			} 

			PlaceItem(GetCenter());
		}
	}
}
