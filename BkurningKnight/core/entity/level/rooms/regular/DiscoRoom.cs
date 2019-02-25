using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class DiscoRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			Painter.Fill(Level, this, 2, Terrain.DISCO);

			if (Random.Chance(50)) {
				if (Random.Chance(50)) {
					Painter.Fill(Level, this, 3, Terrain.RandomFloor());
				} else {
					Painter.Fill(Level, this, 3, Terrain.RandomFloor());
				}


				if (Random.Chance(50)) {
					Painter.Fill(Level, this, 4, Terrain.DISCO);
				} else {
					Painter.FillEllipse(Level, this, 4, Terrain.DISCO);
				}

			} 
		}
	}
}
