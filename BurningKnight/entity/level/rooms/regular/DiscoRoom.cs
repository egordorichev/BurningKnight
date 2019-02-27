using BurningKnight.entity.level.painters;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.regular {
	public class DiscoRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);
			Painter.Fill(Level, this, 2, Terrain.DISCO);

			if (Random.Chance(50)) {
				if (Random.Chance(50))
					Painter.Fill(Level, this, 3, Terrain.RandomFloor());
				else
					Painter.Fill(Level, this, 3, Terrain.RandomFloor());


				if (Random.Chance(50))
					Painter.Fill(Level, this, 4, Terrain.DISCO);
				else
					Painter.FillEllipse(Level, this, 4, Terrain.DISCO);
			}
		}
	}
}