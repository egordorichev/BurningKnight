using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class BigHoleRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(50)) {
				Painter.Fill(Level, this, Random.NewInt(4, 8), Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA);
			} else {
				Painter.FillEllipse(Level, this, Random.NewInt(4, 8), Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA);
			}

		}
	}
}
