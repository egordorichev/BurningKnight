using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class RectCornerRoom : RegularRoom {
		public override Void Paint(Level Level) {
			byte F = Terrain.RandomFloor();
			Painter.Fill(Level, this, Terrain.WALL);
			bool Chasm = Random.Chance(50);

			if (Chasm) {
				Painter.Fill(Level, this, 1, Terrain.CHASM);
			} 

			Painter.Fill(Level, this, 2, F);
			Painter.Fill(Level, this, 3, Chasm ? Terrain.CHASM : Terrain.WALL);

			if (Random.Chance(50)) {
				Painter.FillEllipse(Level, new Rect(this.Left + 1, this.Top + 1, this.Left + 4, this.Top + 4), F);
				Painter.FillEllipse(Level, new Rect(this.Right - 3, this.Top + 1, this.Right, this.Top + 4), F);
				Painter.FillEllipse(Level, new Rect(this.Left + 1, this.Bottom - 3, this.Left + 4, this.Bottom), F);
				Painter.FillEllipse(Level, new Rect(this.Right - 3, this.Bottom - 3, this.Right, this.Bottom), F);
			} else {
				Painter.Fill(Level, new Rect(this.Left + 1, this.Top + 1, this.Left + 4, this.Top + 4), F);
				Painter.Fill(Level, new Rect(this.Right - 3, this.Top + 1, this.Right, this.Top + 4), F);
				Painter.Fill(Level, new Rect(this.Left + 1, this.Bottom - 3, this.Left + 4, this.Bottom), F);
				Painter.Fill(Level, new Rect(this.Right - 3, this.Bottom - 3, this.Right, this.Bottom), F);
			}


			foreach (LDoor Door in Connected.Values()) {
				Door.SetType(LDoor.Type.REGULAR);
			}
		}

		public override bool CanConnect(Point P) {
			if (!(P.X == this.Left + 2 && P.Y == this.Top) && !(P.X == this.Left && P.Y == this.Top + 2) && !(P.X == this.Right - 2 && P.Y == this.Top) && !(P.X == this.Right && P.Y == this.Top + 2) && !(P.X == this.Left + 2 && P.Y == this.Bottom) && !(P.X == this.Left && P.Y == this.Bottom - 2) && !(P.X == this.Right - 2 && P.Y == this.Bottom) && !(P.X == this.Right && P.Y == this.Bottom - 2)) {
				return false;
			} 

			return base.CanConnect(P);
		}
	}
}
