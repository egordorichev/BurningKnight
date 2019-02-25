using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.rooms.connection {
	public class ChasmConnectionRoom : ConnectionRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.RandomFloor());

			if (Random.Chance(50)) {
				Painter.Fill(Level, this, 2, Random.Chance(50) ? Terrain.WALL : Terrain.CHASM);

				if (Random.Chance(50)) {
					if (Random.Chance(50)) {
						Painter.Fill(Level, this, 3, Random.Chance(50) ? Terrain.RandomFloor() : (Random.Chance(50) ? Terrain.WALL : Terrain.CHASM));
					} else {
						Painter.FillEllipse(Level, this, 3, Random.Chance(50) ? Terrain.RandomFloor() : (Random.Chance(50) ? Terrain.WALL : Terrain.CHASM));
					}


					if (Random.Chance(50)) {
						Painter.FillEllipse(Level, this, 4, Random.Chance(50) ? Terrain.RandomFloor() : (Random.Chance(50) ? Terrain.WALL : Terrain.CHASM));
					} 
				} 
			} else {
				Painter.FillEllipse(Level, this, 2, Random.Chance(50) ? Terrain.WALL : Terrain.CHASM);

				if (Random.Chance(50)) {
					Painter.Fill(Level, this, 3, Random.Chance(50) ? Terrain.RandomFloor() : (Random.Chance(50) ? Terrain.WALL : Terrain.CHASM));
				} 

				if (Random.Chance(50)) {
					Painter.FillEllipse(Level, this, 4, Random.Chance(50) ? Terrain.RandomFloor() : (Random.Chance(50) ? Terrain.WALL : Terrain.CHASM));
				} 
			}


			PaintTunnel(Level, Terrain.RandomFloor());
		}

		public override int GetMinWidth() {
			return 5;
		}

		public override int GetMinHeight() {
			return 5;
		}
	}
}
