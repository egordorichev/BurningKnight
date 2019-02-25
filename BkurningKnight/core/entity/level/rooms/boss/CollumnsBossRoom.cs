using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.boss {
	public class CollumnsBossRoom : BossRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			int MinDim = Math.Min(GetWidth(), GetHeight());
			bool Circ = Random.Chance(50);
			byte Tile = Random.Chance(50) ? Terrain.WALL : (Random.Chance(60) ? Terrain.CHASM : Terrain.LAVA);
			int PillarInset = MinDim >= 12 ? 2 : 1;
			int PillarSize = (MinDim - 6) / (PillarInset + 1) - 2;
			float XSpaces = GetWidth() - 2 * PillarInset - PillarSize - 2;
			float YSpaces = GetHeight() - 2 * PillarInset - PillarSize - 2;
			float MinSpaces = Math.Min(XSpaces, YSpaces);
			float PercentSkew = Math.Round(Random.NewFloat() * MinSpaces) / MinSpaces;

			if (Circ) {
				Painter.FillEllipse(Level, Left + 1 + PillarInset + Math.Round(PercentSkew * XSpaces), Top + 1 + PillarInset, PillarSize, PillarSize, Tile);
				Painter.FillEllipse(Level, Right - PillarSize - PillarInset, Top + 1 + PillarInset + Math.Round(PercentSkew * YSpaces), PillarSize, PillarSize, Tile);
				Painter.FillEllipse(Level, Right - PillarSize - PillarInset - Math.Round(PercentSkew * XSpaces), Bottom - PillarSize - PillarInset, PillarSize, PillarSize, Tile);
				Painter.FillEllipse(Level, Left + 1 + PillarInset, Bottom - PillarSize - PillarInset - Math.Round(PercentSkew * YSpaces), PillarSize, PillarSize, Tile);

				if (Random.Chance(50)) {
					Tile = Random.Chance(50) ? Terrain.WALL : (Random.Chance(60) ? Terrain.CHASM : Terrain.LAVA);
					Painter.FillEllipse(Level, Rect.Make(Left + 1 + PillarInset + Math.Round(PercentSkew * XSpaces), Top + 1 + PillarInset, PillarSize, PillarSize), 1, Tile);
					Painter.FillEllipse(Level, Rect.Make(Right - PillarSize - PillarInset, Top + 1 + PillarInset + Math.Round(PercentSkew * YSpaces), PillarSize, PillarSize), 1, Tile);
					Painter.FillEllipse(Level, Rect.Make(Right - PillarSize - PillarInset - Math.Round(PercentSkew * XSpaces), Bottom - PillarSize - PillarInset, PillarSize, PillarSize), 1, Tile);
					Painter.FillEllipse(Level, Rect.Make(Left + 1 + PillarInset, Bottom - PillarSize - PillarInset - Math.Round(PercentSkew * YSpaces), PillarSize, PillarSize), 1, Tile);
				} 
			} else {
				Painter.Fill(Level, Left + 1 + PillarInset + Math.Round(PercentSkew * XSpaces), Top + 1 + PillarInset, PillarSize, PillarSize, Tile);
				Painter.Fill(Level, Right - PillarSize - PillarInset, Top + 1 + PillarInset + Math.Round(PercentSkew * YSpaces), PillarSize, PillarSize, Tile);
				Painter.Fill(Level, Right - PillarSize - PillarInset - Math.Round(PercentSkew * XSpaces), Bottom - PillarSize - PillarInset, PillarSize, PillarSize, Tile);
				Painter.Fill(Level, Left + 1 + PillarInset, Bottom - PillarSize - PillarInset - Math.Round(PercentSkew * YSpaces), PillarSize, PillarSize, Tile);

				if (Random.Chance(50)) {
					Tile = Random.Chance(50) ? Terrain.WALL : (Random.Chance(60) ? Terrain.CHASM : Terrain.LAVA);
					Painter.Fill(Level, Rect.Make(Left + 1 + PillarInset + Math.Round(PercentSkew * XSpaces), Top + 1 + PillarInset, PillarSize, PillarSize), 1, Tile);
					Painter.Fill(Level, Rect.Make(Right - PillarSize - PillarInset, Top + 1 + PillarInset + Math.Round(PercentSkew * YSpaces), PillarSize, PillarSize), 1, Tile);
					Painter.Fill(Level, Rect.Make(Right - PillarSize - PillarInset - Math.Round(PercentSkew * XSpaces), Bottom - PillarSize - PillarInset, PillarSize, PillarSize), 1, Tile);
					Painter.Fill(Level, Rect.Make(Left + 1 + PillarInset, Bottom - PillarSize - PillarInset - Math.Round(PercentSkew * YSpaces), PillarSize, PillarSize), 1, Tile);
				} 
			}

		}
	}
}
