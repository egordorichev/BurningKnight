using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class ChasmRoom : RegularRoom {
		public override Void Paint(Level Level) {
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.RandomFloor());

			if (Random.Chance(50)) {
				if (Random.Chance(50)) {
					Painter.Fill(Level, this, Random.NewInt(1, 5), Terrain.RandomFloor());
				} else {
					Painter.FillEllipse(Level, this, Random.NewInt(1, 5), Terrain.RandomFloor());
				}


				if (Random.Chance(50)) {
					if (Random.Chance(50)) {
						Painter.Fill(Level, this, Random.NewInt(5, 10), Terrain.RandomFloor());
					} else {
						Painter.FillEllipse(Level, this, Random.NewInt(5, 10), Terrain.RandomFloor());
					}

				} 
			} 

			int W = this.GetWidth() - 2;
			bool[] Patch = Patch.Generate(W, this.GetHeight() - 2, 0.6f, 4);

			for (int X = 2; X < this.GetWidth() - 2; X++) {
				for (int Y = 2; Y < this.GetHeight() - 2; Y++) {
					if (Patch[X + Y * W]) {
						Painter.Set(Level, this.Left + X, this.Top + Y, Terrain.CHASM);
					} 
				}
			}

			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.REGULAR);
			}
		}
	}
}
