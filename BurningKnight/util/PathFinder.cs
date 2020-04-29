using System;
using BurningKnight.level.tile;
using BurningKnight.state;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.util {
	public class PathFinder {
		public static int[] Distance;
		public static int[] Neighbours4;
		public static int[] Neighbours8;
		public static int[] Neighbours9;
		public static int[] Circle4;
		public static int[] Corner;
		public static Vector2[] VCircle4 = new[] {new Vector2(0, -1), new Vector2(1, 0), new Vector2(0, 1), new Vector2(-1, 0)};
		public static Vector2[] VCorner = new[] {new Vector2(1, -1), new Vector2(1, 1), new Vector2(-1, 1), new Vector2(-1, -1)};
		public static int[] Circle8;
		private static int[] MaxVal;
		private static bool[] Goals;
		private static int[] Queue;
		private static int Size;
		private static int Width;
		private static int[] Dir;
		private static int[] DirLR;

		public static void SetMapSize(int Width, int Height) {
			PathFinder.Width = Width;
			Size = Width * Height;
			Distance = new int[Size];
			Goals = new bool[Size];
			Queue = new int[(int) (Size * 1.5f)];
			MaxVal = new int[Size];

			for (var i = 0; i < Size; i++) {
				MaxVal[i] = int.MaxValue;
			}

			Dir = new[] {-1, +1, -Width, +Width};
			DirLR = new[] {-1, -Width, +Width, +1};
			Neighbours4 = new[] {-Width, -1, +1, +Width};
			Neighbours8 = new[] {-Width - 1, -Width, -Width + 1, -1, +1, +Width - 1, +Width, +Width + 1};
			Neighbours9 = new[] {-Width - 1, -Width, -Width + 1, -1, 0, +1, +Width - 1, +Width, +Width + 1};
			Circle4 = new[] {-Width, +1, +Width, -1};
			Corner = new[] {-Width + 1, +Width + 1, +Width - 1, -1 - Width};
			Circle8 = new[] {-Width - 1, -Width, -Width + 1, +1, +Width + 1, +Width, +Width - 1, -1};
		}

		public static int StepCost(int I) {
			if ((Tile) Run.Level.Liquid[I] == Tile.Cobweb) {
				return 9;
			}

			return 0;
		}

		public static int GetStep(int From, int To, bool[] Passable) {
			if (!BuildDistanceMap(From, To, Passable)) {
				return -1;
			}

			var MinD = Distance[From];
			var Best = From;
			int Step;
			int StepD;

			for (var I = 0; I < Dir.Length; I++) {
				if ((StepD = Distance[Step = From + Dir[I]]) < MinD) {
					MinD = StepD;
					Best = Step;
				}
			}

			return Best;
		}

		public static int GetStepBack(int Cur, int From, bool[] Passable, int Last) {
			var D = BuildEscapeDistanceMap(Cur, From, 2f, Passable);

			if (Last > -1) {
				Distance[Last] = Int32.MaxValue;
			}

			for (var I = 0; I < Size; I++) {
				Goals[I] = Distance[I] == D;
			}

			if (!BuildDistanceMap(Cur, Goals, Passable)) {
				return -1;
			}

			var MinD = Distance[Cur];
			var Mins = Cur;

			for (var I = 0; I < Dir.Length; I++) {
				var N = Cur + Dir[I];
				var ThisD = Distance[N];

				if (N != Last && Passable[N] && ThisD < MinD) {
					MinD = ThisD;
					Mins = N;
				}
			}

			return Mins;
		}

		private static bool BuildDistanceMap(int From, int To, bool[] Passable) {
			if (From == To) {
				return false;
			}

			for (var i = 0; i < MaxVal.Length; i++) {
				Distance[i] = MaxVal[i];
			}

			var PathFound = false;
			var Head = 0;
			var Tail = 0;

			if (To < 0) {
				return false;
			}

			Queue[Tail++] = To;
			Distance[To] = 0;

			while (Head < Tail) {
				var Step = Queue[Head++];

				if (Step == From) {
					PathFound = true;

					break;
				}

				var NextDistance = Distance[Step] + 1;
				var Start = Step % Width == 0 ? 3 : 0;
				var End = (Step + 1) % Width == 0 ? 3 : 0;

				for (var I = Start; I < DirLR.Length - End; I++) {
					var N = Step + DirLR[I];

					if (N == From || N >= 0 && N < Size && Passable[N] &&
					    Distance[N] > NextDistance) {
						Queue[Tail++] = N;
						Distance[N] = NextDistance + StepCost(N);
					}
				}
			}

			return PathFound;
		}

		private static bool BuildDistanceMap(int From, bool[] To, bool[] Passable) {
			if (To[From]) {
				return false;
			}

			for (var i = 0; i < MaxVal.Length; i++) {
				Distance[i] = MaxVal[i];
			}

			var PathFound = false;
			var Head = 0;
			var Tail = 0;

			for (var I = 0; I < Size; I++) {
				if (To[I]) {
					Queue[Tail++] = I;
					Distance[I] = 0;
				}
			}

			while (Head < Tail) {
				var Step = Queue[Head++];

				if (Step == From) {
					PathFound = true;

					break;
				}

				var NextDistance = Distance[Step] + 1;
				var Start = Step % Width == 0 ? 3 : 0;
				var End = (Step + 1) % Width == 0 ? 3 : 0;

				for (var I = Start; I < DirLR.Length - End; I++) {
					var N = Step + DirLR[I];

					if (N == From || N >= 0 && N < Size && Passable[N] &&
					    Distance[N] > NextDistance) {
						Queue[Tail++] = N;
						Distance[N] = NextDistance + StepCost(N);
					}
				}
			}

			return PathFound;
		}

		private static int BuildEscapeDistanceMap(int Cur, int From, float Factor, bool[] Passable) {
			for (var i = 0; i < MaxVal.Length; i++) {
				Distance[i] = MaxVal[i];
			}

			var DestDist = int.MaxValue;

			var Head = 0;
			var Tail = 0;
			Queue[Tail++] = From;
			Distance[From] = 0;
			var Dist = 0;

			while (Head < Tail) {
				var Step = Queue[Head++];
				Dist = Distance[Step];

				if (Dist > DestDist) {
					return DestDist;
				}

				if (Step == Cur) {
					DestDist = (int) (Dist * Factor) + 1;
				}

				var NextDistance = Dist + 1;
				var Start = Step % Width == 0 ? 3 : 0;
				var End = (Step + 1) % Width == 0 ? 3 : 0;

				for (var I = Start; I < DirLR.Length - End; I++) {
					var N = Step + DirLR[I];

					if (N >= 0 && N < Size && Passable[N] && Distance[N] > NextDistance) {
						Queue[Tail++] = N;
						Distance[N] = NextDistance + StepCost(N);
					}
				}
			}

			return Dist;
		}

		public static void BuildDistanceMap(int To, bool[] Passable) {
			for (var i = 0; i < MaxVal.Length; i++) {
				Distance[i] = MaxVal[i];
			}

			var Head = 0;
			var Tail = 0;
			Queue[Tail++] = To;
			Distance[To] = 0;

			while (Head < Tail) {
				var Step = Queue[Head++];
				var NextDistance = Distance[Step] + 1;
				var Start = Step % Width == 0 ? 3 : 0;
				var End = (Step + 1) % Width == 0 ? 3 : 0;

				for (var I = Start; I < DirLR.Length - End; I++) {
					var N = Step + DirLR[I];

					if (N >= 0 && N < Size && Passable[N] && Distance[N] > NextDistance) {
						Queue[Tail++] = N;
						Distance[N] = NextDistance + StepCost(N);
					}
				}
			}
		}
	}
}