using BurningKnight.entity.level.painters;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.boss {
	public class CollumnsBossRoomDef : BossRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);
			int MinDim = Math.Min(GetWidth(), GetHeight());
			var Circ = Random.Chance(50);
			var Tile = Random.Chance(50) ? Terrain.WALL : (Random.Chance(60) ? Terrain.CHASM : Terrain.LAVA);
			var PillarInset = MinDim >= 12 ? 2 : 1;
			var PillarSize = (MinDim - 6) / (PillarInset + 1) - 2;
			float XSpaces = GetWidth() - 2 * PillarInset - PillarSize - 2;
			float YSpaces = GetHeight() - 2 * PillarInset - PillarSize - 2;
			float MinSpaces = Math.Min(XSpaces, YSpaces);
			var PercentSkew = Math.Round(Random.NewFloat() * MinSpaces) / MinSpaces;

			if (Circ) {
				Painter.FillEllipse(Level, Left + 1 + PillarInset + Math.Round(PercentSkew * XSpaces), Top + 1 + PillarInset, PillarSize, PillarSize, Tile);
				Painter.FillEllipse(Level, Right - PillarSize - PillarInset, Top + 1 + PillarInset + Math.Round(PercentSkew * YSpaces), PillarSize, PillarSize, Tile);
				Painter.FillEllipse(Level, Right - PillarSize - PillarInset - Math.Round(PercentSkew * XSpaces), Bottom - PillarSize - PillarInset, PillarSize, PillarSize, Tile);
				Painter.FillEllipse(Level, Left + 1 + PillarInset, Bottom - PillarSize - PillarInset - Math.Round(PercentSkew * YSpaces), PillarSize, PillarSize, Tile);

				if (Random.Chance(50)) {
					Tile = Random.Chance(50) ? Terrain.WALL : (Random.Chance(60) ? Terrain.CHASM : Terrain.LAVA);
					Painter.FillEllipse(Level, Make(Left + 1 + PillarInset + Math.Round(PercentSkew * XSpaces), Top + 1 + PillarInset, PillarSize, PillarSize), 1, Tile);
					Painter.FillEllipse(Level, Make(Right - PillarSize - PillarInset, Top + 1 + PillarInset + Math.Round(PercentSkew * YSpaces), PillarSize, PillarSize), 1, Tile);
					Painter.FillEllipse(Level, Make(Right - PillarSize - PillarInset - Math.Round(PercentSkew * XSpaces), Bottom - PillarSize - PillarInset, PillarSize, PillarSize), 1, Tile);
					Painter.FillEllipse(Level, Make(Left + 1 + PillarInset, Bottom - PillarSize - PillarInset - Math.Round(PercentSkew * YSpaces), PillarSize, PillarSize), 1, Tile);
				}
			}
			else {
				Painter.Fill(Level, Left + 1 + PillarInset + Math.Round(PercentSkew * XSpaces), Top + 1 + PillarInset, PillarSize, PillarSize, Tile);
				Painter.Fill(Level, Right - PillarSize - PillarInset, Top + 1 + PillarInset + Math.Round(PercentSkew * YSpaces), PillarSize, PillarSize, Tile);
				Painter.Fill(Level, Right - PillarSize - PillarInset - Math.Round(PercentSkew * XSpaces), Bottom - PillarSize - PillarInset, PillarSize, PillarSize, Tile);
				Painter.Fill(Level, Left + 1 + PillarInset, Bottom - PillarSize - PillarInset - Math.Round(PercentSkew * YSpaces), PillarSize, PillarSize, Tile);

				if (Random.Chance(50)) {
					Tile = Random.Chance(50) ? Terrain.WALL : (Random.Chance(60) ? Terrain.CHASM : Terrain.LAVA);
					Painter.Fill(Level, Make(Left + 1 + PillarInset + Math.Round(PercentSkew * XSpaces), Top + 1 + PillarInset, PillarSize, PillarSize), 1, Tile);
					Painter.Fill(Level, Make(Right - PillarSize - PillarInset, Top + 1 + PillarInset + Math.Round(PercentSkew * YSpaces), PillarSize, PillarSize), 1, Tile);
					Painter.Fill(Level, Make(Right - PillarSize - PillarInset - Math.Round(PercentSkew * XSpaces), Bottom - PillarSize - PillarInset, PillarSize, PillarSize), 1, Tile);
					Painter.Fill(Level, Make(Left + 1 + PillarInset, Bottom - PillarSize - PillarInset - Math.Round(PercentSkew * YSpaces), PillarSize, PillarSize), 1, Tile);
				}
			}
		}
	}
}