using BurningKnight.entity.level.painters;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.treasure {
	public class MazeTreasureRoom : TreasureRoom {
		private static byte[] Types = {Terrain.WALL, Terrain.CHASM};
		private static float[] Chanches = {1, 1f};

		public override void Paint(Level Level) {
			base.Paint(Level);
			var Wall = Types[Random.Chances(Chanches)];
			bool[][] Maze = Maze.Generate(this);
			Painter.Fill(Level, this, Terrain.WALL);

			if (Random.Chance(50)) {
				Painter.Fill(Level, this, 1, Terrain.FLOOR_D);
			}
			else {
				Painter.Fill(Level, this, 1, Terrain.RandomFloor());
				Painter.FillEllipse(Level, this, 1, Terrain.FLOOR_D);
			}


			if (Random.Chance(50)) {
				if (Random.Chance(50))
					Painter.Fill(Level, this, Random.NewInt(2, 5), Terrain.RandomFloor());
				else
					Painter.FillEllipse(Level, this, Random.NewInt(2, 5), Terrain.RandomFloor());
			}

			for (var X = 0; X < GetWidth(); X++)
			for (var Y = 0; Y < GetHeight(); Y++)
				if (Maze[X][Y] == Maze.FILLED)
					Painter.Set(Level, Left + X, Top + Y, X == 0 || Y == 0 || X == GetWidth() - 1 || Y == GetHeight() - 1 ? Terrain.WALL : Wall);

			var Center = GetCenter();

			if (Random.Chance(50))
				Painter.Fill(Level, (int) Center.X - 2, (int) Center.Y - 2, 5, 5, Random.Chance(50) ? Terrain.FLOOR_B : Terrain.FLOOR_D);
			else
				Painter.FillEllipse(Level, (int) Center.X - 2, (int) Center.Y - 2, 5, 5, Random.Chance(50) ? Terrain.FLOOR_B : Terrain.FLOOR_D);


			PlaceChest(Center);
		}

		public override int GetMinWidth() {
			return 13;
		}

		public override int GetMinHeight() {
			return 13;
		}

		public override int GetMaxHeight() {
			return 16;
		}

		public override int GetMaxWidth() {
			return 16;
		}
	}
}