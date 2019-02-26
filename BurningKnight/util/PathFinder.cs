using System;
using BurningKnight.entity.level;

namespace BurningKnight.util {
	public class PathFinder {
		public static int[] Distance;
		public static int[] NEIGHBOURS4;
		public static int[] NEIGHBOURS8;
		public static int[] NEIGHBOURS9;
		public static int[,] Check;
		public static int[][] CheckLR;
		public static int[] CIRCLE4;
		public static int[] CIRCLE8;
		private static int[] MaxVal;
		private static bool[] Goals;
		private static int[] Queue;
		private static int Size;
		private static int Width;
		private static int[] Dir;
		private static int[] DirLR;
		public static int LastStep;

		public static void SetMapSize(int Width, int Height) {
			PathFinder.Width = Width;
			Size = Width * Height;
			Distance = new int[Size];
			Goals = new bool[Size];
			Queue = new int[Size];
			MaxVal = new int[Size];
			for (int i = 0; i < Size; i++) {
				MaxVal[i] = Int32.MaxValue;				
			}
			
			Dir = new[] {
				-1, +1, -Width, +Width
			}
			;
			DirLR = new[] {
				-1, -Width, +Width, +1
			}
			;
			NEIGHBOURS4 = new[] {
				-Width, -1, +1, +Width
			}
			;
			NEIGHBOURS8 = new[] {
				-Width - 1, -Width, -Width + 1, -1, +1, +Width - 1, +Width, +Width + 1
			}
			;
			NEIGHBOURS9 = new[] {
				-Width - 1, -Width, -Width + 1, -1, 0, +1, +Width - 1, +Width, +Width + 1
			}
			;
			Check = new[,] {
				{ 0, 0
				}, { 0, 0
				}, { 0, 0
				}, { 0, 0
				}, {
					-Width, -1
				}, {
					-Width, 1
				}, {
					Width, -1
				}, {
					Width, 1
				}
			}
			;
			
			CIRCLE4 = new[]  {
				-Width, +1, +Width, -1
			}
			;
			CIRCLE8 = new[]  {
				-Width - 1, -Width, -Width + 1, +1, +Width + 1, +Width, +Width - 1, -1
			}
			;
		}

		public static bool GoodMove(int From, int I, bool[] Passable) {
			return true;
		}

		public static bool GoodMoveLR(int From, int I, bool[] Passable) {
			return true;
		}

		public static int StepCost(int I) {
			byte T = Dungeon.Level.LiquidData[I];

			if (T == Terrain.COBWEB) return 9;

			return 0;
		}

		public static int GetStep(int From, int To, bool[] Passable) {
			if (!BuildDistanceMap(From, To, Passable)) return -1;

			var MinD = Distance[From];
			var Best = From;
			int Step;
			int StepD;

			for (var I = 0; I < Dir.Length; I++)
				if ((StepD = Distance[Step = From + Dir[I]]) < MinD && GoodMove(Step, I, Passable)) {
					MinD = StepD;
					Best = Step;
				}

			return Best;
		}

		public static int GetStepBack(int Cur, int From, bool[] Passable, int Last) {
			var D = BuildEscapeDistanceMap(Cur, From, 2f, Passable);

			for (var I = 0; I < Size; I++) Goals[I] = Distance[I] == D;

			if (!BuildDistanceMap(Cur, Goals, Passable)) return -1;

			var MinD = Distance[Cur];
			var Mins = Cur;

			for (var I = 0; I < Dir.Length; I++) {
				var N = Cur + Dir[I];
				var ThisD = Distance[N];

				if (N != Last && GoodMove(From, I, Passable) && ThisD < MinD) {
					MinD = ThisD;
					Mins = N;
				}
			}

			return Mins;
		}

		private static bool BuildDistanceMap(int From, int To, bool[] Passable) {
			if (From == To) return false;

			for (int i = 0; i < MaxVal.Length; i++) {
				Distance[i] = MaxVal[i];
			}
			
			var PathFound = false;
			var Head = 0;
			var Tail = 0;

			if (To < 0) return false;

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

					if (N == From || N >= 0 && N < Size && Passable[N] && GoodMoveLR(Step, I, Passable) && Distance[N] > NextDistance) {
						Queue[Tail++] = N;
						Distance[N] = NextDistance + StepCost(N);
					}
				}
			}

			return PathFound;
		}

		private static bool BuildDistanceMap(int From, bool[] To, bool[] Passable) {
			if (To[From]) return false;

			for (int i = 0; i < MaxVal.Length; i++) {
				Distance[i] = MaxVal[i];
			}
			
			var PathFound = false;
			var Head = 0;
			var Tail = 0;

			for (var I = 0; I < Size; I++)
				if (To[I]) {
					Queue[Tail++] = I;
					Distance[I] = 0;
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

					if (N == From || N >= 0 && N < Size && Passable[N] && GoodMoveLR(Step, I, Passable) && Distance[N] > NextDistance) {
						Queue[Tail++] = N;
						Distance[N] = NextDistance + StepCost(N);
					}
				}
			}

			return PathFound;
		}

		private static int BuildEscapeDistanceMap(int Cur, int From, float Factor, bool[] Passable) {
			for (int i = 0; i < MaxVal.Length; i++) {
				Distance[i] = MaxVal[i];
			}
			
			int DestDist = Int32.MaxValue;
		
			var Head = 0;
			var Tail = 0;
			Queue[Tail++] = From;
			Distance[From] = 0;
			var Dist = 0;

			while (Head < Tail) {
				var Step = Queue[Head++];
				Dist = Distance[Step];

				if (Dist > DestDist) return DestDist;

				if (Step == Cur) DestDist = (int) (Dist * Factor) + 1;

				var NextDistance = Dist + 1;
				var Start = Step % Width == 0 ? 3 : 0;
				var End = (Step + 1) % Width == 0 ? 3 : 0;

				for (var I = Start; I < DirLR.Length - End; I++) {
					var N = Step + DirLR[I];

					if (N >= 0 && N < Size && Passable[N] && GoodMoveLR(Step, I, Passable) && Distance[N] > NextDistance) {
						Queue[Tail++] = N;
						Distance[N] = NextDistance + StepCost(N);
						LastStep = N;
					}
				}
			}

			return Dist;
		}

		public static void BuildDistanceMap(int To, bool[] Passable) {
			for (int i = 0; i < MaxVal.Length; i++) {
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

					if (N >= 0 && N < Size && Passable[N] && GoodMoveLR(Step, I, Passable) && Distance[N] > NextDistance) {
						Queue[Tail++] = N;
						Distance[N] = NextDistance + StepCost(N);
					}
				}
			}
		}
	}
}