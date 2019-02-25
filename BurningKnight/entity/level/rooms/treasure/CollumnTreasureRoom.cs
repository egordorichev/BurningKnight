using BurningKnight.entity.level.painters;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.treasure {
	public class CollumnTreasureRoom : TreasureRoom {
		public override void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(50))
				Painter.Fill(Level, this, 1, Terrain.FLOOR_D);
			else
				Painter.FillEllipse(Level, this, 1, Terrain.FLOOR_D);


			if (Random.Chance(50)) {
				var M = Random.NewInt(3, 6);

				if (Random.Chance(50))
					Painter.Fill(Level, this, M, Terrain.FLOOR_B);
				else
					Painter.FillEllipse(Level, this, M, Terrain.FLOOR_B);


				if (Random.Chance(50)) {
					M += Random.NewInt(1, 3);

					if (Random.Chance(50))
						Painter.Fill(Level, this, M, Terrain.FLOOR_D);
					else
						Painter.FillEllipse(Level, this, M, Terrain.FLOOR_D);
				}
			}

			int MinDim = Math.Min(GetWidth(), GetHeight());
			var Circ = Random.Chance(50);

			if (MinDim == 7 || Random.NewInt(2) == 0) {
				var PillarInset = MinDim >= 11 ? 2 : 1;
				var PillarSize = (MinDim - 3) / 2 - PillarInset;
				int PillarX;
				int PillarY;

				if (Random.NewInt(2) == 0) {
					PillarX = Random.NewInt(Left + 1 + PillarInset, Right - PillarSize - PillarInset);
					PillarY = Top + 1 + PillarInset;
				}
				else {
					PillarX = Left + 1 + PillarInset;
					PillarY = Random.NewInt(Top + 1 + PillarInset, Bottom - PillarSize - PillarInset);
				}


				if (Circ)
					Painter.FillEllipse(Level, PillarX, PillarY, PillarSize, PillarSize, Terrain.WALL);
				else
					Painter.Fill(Level, PillarX, PillarY, PillarSize, PillarSize, Terrain.WALL);


				PillarX = Right - (PillarX - Left + PillarSize - 1);
				PillarY = Bottom - (PillarY - Top + PillarSize - 1);

				if (Circ)
					Painter.FillEllipse(Level, PillarX, PillarY, PillarSize, PillarSize, Terrain.WALL);
				else
					Painter.Fill(Level, PillarX, PillarY, PillarSize, PillarSize, Terrain.WALL);
			}
			else {
				var PillarInset = MinDim >= 12 ? 2 : 1;
				var PillarSize = (MinDim - 6) / (PillarInset + 1);
				float XSpaces = GetWidth() - 2 * PillarInset - PillarSize - 2;
				float YSpaces = GetHeight() - 2 * PillarInset - PillarSize - 2;
				float MinSpaces = Math.Min(XSpaces, YSpaces);
				var PercentSkew = Math.Round(Random.NewFloat() * MinSpaces) / MinSpaces;

				if (Circ) {
					Painter.FillEllipse(Level, Left + 1 + PillarInset + Math.Round(PercentSkew * XSpaces), Top + 1 + PillarInset, PillarSize, PillarSize, Terrain.WALL);
					Painter.FillEllipse(Level, Right - PillarSize - PillarInset, Top + 1 + PillarInset + Math.Round(PercentSkew * YSpaces), PillarSize, PillarSize, Terrain.WALL);
					Painter.FillEllipse(Level, Right - PillarSize - PillarInset - Math.Round(PercentSkew * XSpaces), Bottom - PillarSize - PillarInset, PillarSize, PillarSize, Terrain.WALL);
					Painter.FillEllipse(Level, Left + 1 + PillarInset, Bottom - PillarSize - PillarInset - Math.Round(PercentSkew * YSpaces), PillarSize, PillarSize, Terrain.WALL);
				}
				else {
					Painter.Fill(Level, Left + 1 + PillarInset + Math.Round(PercentSkew * XSpaces), Top + 1 + PillarInset, PillarSize, PillarSize, Terrain.WALL);
					Painter.Fill(Level, Right - PillarSize - PillarInset, Top + 1 + PillarInset + Math.Round(PercentSkew * YSpaces), PillarSize, PillarSize, Terrain.WALL);
					Painter.Fill(Level, Right - PillarSize - PillarInset - Math.Round(PercentSkew * XSpaces), Bottom - PillarSize - PillarInset, PillarSize, PillarSize, Terrain.WALL);
					Painter.Fill(Level, Left + 1 + PillarInset, Bottom - PillarSize - PillarInset - Math.Round(PercentSkew * YSpaces), PillarSize, PillarSize, Terrain.WALL);
				}
			}


			var Center = GetCenter();
			Painter.Fill(Level, (int) Center.X - 1, (int) Center.Y - 1, 3, 3, Terrain.RandomFloor());
			PlaceChest(GetCenter());
		}
	}
}