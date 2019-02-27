using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.regular {
	public class ChasmRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.RandomFloor());

			if (Random.Chance(50)) {
				if (Random.Chance(50))
					Painter.Fill(Level, this, Random.NewInt(1, 5), Terrain.RandomFloor());
				else
					Painter.FillEllipse(Level, this, Random.NewInt(1, 5), Terrain.RandomFloor());


				if (Random.Chance(50)) {
					if (Random.Chance(50))
						Painter.Fill(Level, this, Random.NewInt(5, 10), Terrain.RandomFloor());
					else
						Painter.FillEllipse(Level, this, Random.NewInt(5, 10), Terrain.RandomFloor());
				}
			}

			var W = GetWidth() - 2;
			bool[] Patch = Patch.Generate(W, GetHeight() - 2, 0.6f, 4);

			for (var X = 2; X < GetWidth() - 2; X++)
			for (var Y = 2; Y < GetHeight() - 2; Y++)
				if (Patch[X + Y * W])
					Painter.Set(Level, Left + X, Top + Y, Terrain.CHASM);

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.REGULAR);
		}
	}
}