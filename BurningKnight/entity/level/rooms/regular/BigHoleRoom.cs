using BurningKnight.entity.level.painters;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.regular {
	public class BigHoleRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(50))
				Painter.Fill(Level, this, Random.NewInt(4, 8), Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA);
			else
				Painter.FillEllipse(Level, this, Random.NewInt(4, 8), Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA);
		}
	}
}