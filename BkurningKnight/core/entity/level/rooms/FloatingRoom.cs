using BurningKnight.core.entity.level.entities;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.entity.level.rooms.regular;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms {
	public class FloatingRoom : RegularRoom {
		private Level Level;
		private float TurnChance = 20;
		private float Chance2 = 30;
		private float Chance3 = 10;
		private int NumDiggers;
		private float DiggerChance = 5;
		private static Vector2[] Dirs = { new Vector2(0, 1), new Vector2(0, -1), new Vector2(1, 0), new Vector2(-1, 0) };

		public override int GetMinWidth() {
			return 64;
		}

		public override int GetMaxWidth() {
			return 65;
		}

		public override int GetMinHeight() {
			return 64;
		}

		public override int GetMaxHeight() {
			return 65;
		}

		public override Void Paint(Level Level) {
			this.Level = Level;
			Painter.Fill(Level, this, Terrain.WALL);
			NewDigger(31, 31);
			Entrance Entrance = new Entrance();
			Entrance.X = 31 * 16 + 1;
			Entrance.Y = 31 * 16 - 6;
			LevelSave.Add(Entrance);
			Dungeon.Area.Add(Entrance);
		}

		private Void Dig(Vector2 Pos) {
			Painter.Set(Level, new Point(Pos.X + Left, Pos.Y + Top), Terrain.RandomFloor());
		}

		private Void Dig(Vector2 Pos, int Size) {
			int X = (int) Pos.X;
			int Y = (int) Pos.Y;
			Painter.Fill(Level, new Rect(X + Left - (int) Math.Ceil(Size / 2f), Y + Top - (int) Math.Ceil(Size / 2f), X + Left + Size / 2, Y + Top + Size / 2), Terrain.RandomFloor());
		}

		private Void NewDigger(int X, int Y) {
			NumDiggers++;
			Vector2 Dir = RandomDir();
			Vector2 Pos = new Vector2(X, Y);
			Dig(Pos, 3);

			for (int I = 0; I < Random.NewInt(64, 128); I++) {
				if (Random.Chance(TurnChance)) {
					Dir = RandomDir(Dir);
				} 

				Pos.X += Dir.X;
				Pos.Y += Dir.Y;

				if (Pos.X < 2 || Pos.Y < 2 || Pos.X > 61 || Pos.Y > 61) {
					return;
				} 

				if (NumDiggers < 6 && Random.Chance(DiggerChance)) {
					NewDigger((int) Pos.X, (int) Pos.Y);
				} 

				if (Random.Chance(Chance2)) {
					Dig(Pos, 2);
				} else if (Random.Chance(Chance3)) {
					Dig(Pos, 3);
				} else {
					Dig(Pos);
				}

			}
		}

		private Vector2 RandomDir() {
			return Dirs[Random.NewInt(4)];
		}

		private Vector2 RandomDir(Vector2 Last) {
			do {
				Vector2 Dir = RandomDir();

				if (Last != Dir) {
					return Dir;
				} 
			} while (true);
		}
	}
}
