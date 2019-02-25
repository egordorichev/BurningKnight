using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class RegularRoom : Room {
		public static class Size {
			public const Size NORMAL = new Size(8, 12, 1);
			public const Size LARGE = new Size(12, 14, 2);
			public const Size GIANT = new Size(14, 16, 3);
			public const Size[] Values = { NORMAL, LARGE, GIANT };
			public const int MinDim;
			public const int MaxDim;
			public const int RoomValue;

			public Size(int Min, int Max, int Val) {
				this.MinDim = Min;
				this.MaxDim = Max;
				this.RoomValue = Val;
			}

			public int GetConnectionWeight() {
				return this.RoomValue * this.RoomValue;
			}
		}

		protected Size Size = Size.NORMAL;

		public override Void Paint(Level Level) {
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.RandomFloor());

			if (Random.Chance(50)) {
				Painter.FillEllipse(Level, this, 1, Terrain.RandomFloor());
			} 

			if (Random.Chance(50)) {
				Painter.Fill(Level, this, Random.NewInt(2, 6), Terrain.RandomFloor());
			} 

			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.ENEMY);
			}
		}

		public bool SetSize(int Min, int Max) {
			if (GameSave.RunId == 0) {
				this.Size = Size.NORMAL;

				return true;
			} 

			float[] Chances = this.GetSizeChance();
			Size[] Sizes = Size.Values;

			if (Chances.Length != Sizes.Length) {
				return false;
			} 

			for (int I = 0; I < Min; I++) {
				Chances[I] = 0;
			}

			for (int I = Max + 1; I < Chances.Length; I++) {
				Chances[I] = 0;
			}

			int Index = Random.Chances(Chances);

			if (Index == -1) {
				this.Size = Sizes[0];

				return false;
			} else {
				return true;
			}

		}

		protected float GetSizeChance() {
			return { 1, 0, 0 };
		}

		public Size GetSize() {
			return this.Size;
		}

		public static RegularRoom Create() {
			if (Dungeon.Depth < 1) {
				return new RegularRoom();
			} 

			return RegularRoomPool.Instance.Generate();
		}

		public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.ALL) {
				return 16;
			} 

			return 4;
		}

		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.ALL) {
				return 1;
			} 

			return 0;
		}
	}
}
