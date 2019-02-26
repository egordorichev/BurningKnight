using BurningKnight.entity.level.painters;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.regular {
	public class CollumnRoom : RegularRoom {
		public override void Paint(Level Level) {
			base.Paint(Level);
			var M = Random.NewInt(4, 8);

			if (Random.Chance(50))
				Painter.FillEllipse(Level, this, M, Random.Chance(50) ? Terrain.CHASM : Terrain.WALL);
			else
				Painter.Fill(Level, this, M, Random.Chance(50) ? Terrain.CHASM : Terrain.WALL);
		}

		public override int GetMinWidth() {
			return 8;
		}

		public override int GetMinHeight() {
			return 8;
		}
	}
}