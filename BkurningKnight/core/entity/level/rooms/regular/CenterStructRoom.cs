using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class CenterStructRoom : RegularRoom {
		public override Void Paint(Level Level) {
			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.REGULAR);
			}

			bool El = Random.Chance(50);
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.CHASM);
			int M = Random.NewInt(2, 4);
			Painter.Fill(Level, this, M, Terrain.RandomFloor());
			M += Random.NewInt(2, 4);
			bool Before = Random.Chance(50);

			if (Before) {
				this.PaintTunnel(Level, Terrain.RandomFloor(), true);
			} 

			Painter.Fill(Level, this, M, GetSolid());
			byte F = Terrain.RandomFloor();
			bool S = false;

			if (Random.Chance(50)) {
				Painter.Set(Level, new Point(this.GetWidth() / 2 + this.Left, this.Top + M), F);
				S = true;
			} 

			if (Random.Chance(50)) {
				Painter.Set(Level, new Point(this.GetWidth() / 2 + this.Left, this.Bottom - M), F);
				S = true;
			} 

			if (Random.Chance(50)) {
				Painter.Set(Level, new Point(this.Left + M, this.GetHeight() / 2 + this.Top), F);
				S = true;
			} 

			if (Random.Chance(50) || !S) {
				Painter.Set(Level, new Point(this.Right - M, this.GetHeight() / 2 + this.Top), F);
			} 

			El = El || Random.Chance(50);

			if (El) {
				M++;
				Painter.FillEllipse(Level, this, M, F);
			} else {
				Painter.Fill(Level, this, M, F);
			}


			if (Random.Chance(50)) {
				M += 1f;

				if (El) {
					M += 1f;
					Painter.FillEllipse(Level, this, M, GetSolid());
				} else {
					Painter.Fill(Level, this, M, GetSolid());
				}

			} 

			if (!Before) {
				this.PaintTunnel(Level, Terrain.RandomFloor(), true);
			} 
		}

		protected override Point GetDoorCenter() {
			return GetCenter();
		}

		private byte GetSolid() {
			return Random.Chance(50) ? Terrain.CHASM : Terrain.WALL;
		}

		public override int GetMinWidth() {
			return 12;
		}

		public override int GetMinHeight() {
			return 12;
		}
	}
}
