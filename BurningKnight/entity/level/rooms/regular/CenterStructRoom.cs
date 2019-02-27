using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class CenterStructRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.REGULAR);

			var El = Random.Chance(50);
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.CHASM);
			var M = Random.NewInt(2, 4);
			Painter.Fill(Level, this, M, Terrain.RandomFloor());
			M += Random.NewInt(2, 4);
			var Before = Random.Chance(50);

			if (Before) PaintTunnel(Level, Terrain.RandomFloor(), true);

			Painter.Fill(Level, this, M, GetSolid());
			var F = Terrain.RandomFloor();
			var S = false;

			if (Random.Chance(50)) {
				Painter.Set(Level, new Point(GetWidth() / 2 + Left, Top + M), F);
				S = true;
			}

			if (Random.Chance(50)) {
				Painter.Set(Level, new Point(GetWidth() / 2 + Left, Bottom - M), F);
				S = true;
			}

			if (Random.Chance(50)) {
				Painter.Set(Level, new Point(Left + M, GetHeight() / 2 + Top), F);
				S = true;
			}

			if (Random.Chance(50) || !S) Painter.Set(Level, new Point(Right - M, GetHeight() / 2 + Top), F);

			El = El || Random.Chance(50);

			if (El) {
				M++;
				Painter.FillEllipse(Level, this, M, F);
			}
			else {
				Painter.Fill(Level, this, M, F);
			}


			if (Random.Chance(50)) {
				M += 1f;

				if (El) {
					M += 1f;
					Painter.FillEllipse(Level, this, M, GetSolid());
				}
				else {
					Painter.Fill(Level, this, M, GetSolid());
				}
			}

			if (!Before) PaintTunnel(Level, Terrain.RandomFloor(), true);
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