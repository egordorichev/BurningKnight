using BurningKnight.core.entity.level;

namespace BurningKnight.core.util {
	public class PathFinder {
		public static int[] Distance;
		public static int[] NEIGHBOURS4;
		public static int[] NEIGHBOURS8;
		public static int[] NEIGHBOURS9;
		public static int[][] Check;
		public static int[][] CheckLR;
		public static int[] CIRCLE4;
		public static int[] CIRCLE8;
		private static int[] MaxVal;
		private static bool[] Goals;
		private static int[] Queue;
		private static int Size = 0;
		private static int Width = 0;
		private static int[] Dir;
		private static int[] DirLR;
		public static int LastStep;

		public static Void SetMapSize(int Width, int Height) {
			PathFinder.Width = Width;
			PathFinder.Size = Width * Height;
			Distance = new int[Size];
			Goals = new bool[Size];
			Queue = new int[Size];
			MaxVal = new int[Size];
			Arrays.Fill(MaxVal, Integer.MAX_VALUE);
			Dir = { -1, +1, -Width, +Width };
			DirLR = { -1, -Width, +Width, +1 };
			NEIGHBOURS4 = { -Width, -1, +1, +Width };
			NEIGHBOURS8 = { -Width - 1, -Width, -Width + 1, -1, +1, +Width - 1, +Width, +Width + 1 };
			NEIGHBOURS9 = { -Width - 1, -Width, -Width + 1, -1, 0, +1, +Width - 1, +Width, +Width + 1 };
			Check = { {}, {}, {}, {}, { -Width, -1 }, { -Width, 1 }, { Width, -1 }, { Width, 1 } };
			CheckLR = { { -1, -Width }, {}, { -1, Width }, {}, {}, { -Width, 1 }, { Width, +1 }, {} };
			CIRCLE4 = { -Width, +1, +Width, -1 };
			CIRCLE8 = { -Width - 1, -Width, -Width + 1, +1, +Width + 1, +Width, +Width - 1, -1 };
		}

		public static bool GoodMove(int From, int I, bool Passable) {
			return true;
		}

		public static bool GoodMoveLR(int From, int I, bool Passable) {
			return true;
		}

		public static int StepCost(int I) {
			byte T = Dungeon.Level.LiquidData[I];

			if (T == Terrain.COBWEB) {
				return 9;
			} 

			return 0;
		}

		public static int GetStep(int From, int To, bool Passable) {
			if (!BuildDistanceMap(From, To, Passable)) {
				return -1;
			} 

			int MinD = Distance[From];
			int Best = From;
			int Step;
			int StepD;

			for (int I = 0; I < Dir.Length; I++) {
				if ((StepD = Distance[Step = From + Dir[I]]) < MinD && GoodMove(Step, I, Passable)) {
					MinD = StepD;
					Best = Step;
				} 
			}

			return Best;
		}

		public static int GetStepBack(int Cur, int From, bool Passable, int Last) {
			int D = BuildEscapeDistanceMap(Cur, From, 2f, Passable);

			for (int I = 0; I < Size; I++) {
				Goals[I] = Distance[I] == D;
			}

			if (!BuildDistanceMap(Cur, Goals, Passable)) {
				return -1;
			} 

			int MinD = Distance[Cur];
			int Mins = Cur;

			for (int I = 0; I < Dir.Length; I++) {
				int N = Cur + Dir[I];
				int ThisD = Distance[N];

				if (N != Last && GoodMove(From, I, Passable) && ThisD < MinD) {
					MinD = ThisD;
					Mins = N;
				} 
			}

			return Mins;
		}

		private static bool BuildDistanceMap(int From, int To, bool Passable) {
			if (From == To) {
				return false;
			} 

			System.Arraycopy(MaxVal, 0, Distance, 0, MaxVal.Length);
			bool PathFound = false;
			int Head = 0;
			int Tail = 0;

			if (To < 0) {
				return false;
			} 

			Queue[Tail++] = To;
			Distance[To] = 0;

			while (Head < Tail) {
				int Step = Queue[Head++];

				if (Step == From) {
					PathFound = true;

					break;
				} 

				int NextDistance = Distance[Step] + 1;
				int Start = (Step % Width == 0 ? 3 : 0);
				int End = ((Step + 1) % Width == 0 ? 3 : 0);

				for (int I = Start; I < DirLR.Length - End; I++) {
					int N = Step + DirLR[I];

					if ((N == From || (N >= 0 && N < Size && Passable[N] && GoodMoveLR(Step, I, Passable) && (Distance[N] > NextDistance)))) {
						Queue[Tail++] = N;
						Distance[N] = NextDistance + StepCost(N);
					} 
				}
			}

			return PathFound;
		}

		private static bool BuildDistanceMap(int From, bool To, bool Passable) {
			if (To[From]) {
				return false;
			} 

			System.Arraycopy(MaxVal, 0, Distance, 0, MaxVal.Length);
			bool PathFound = false;
			int Head = 0;
			int Tail = 0;

			for (int I = 0; I < Size; I++) {
				if (To[I]) {
					Queue[Tail++] = I;
					Distance[I] = 0;
				} 
			}

			while (Head < Tail) {
				int Step = Queue[Head++];

				if (Step == From) {
					PathFound = true;

					break;
				} 

				int NextDistance = Distance[Step] + 1;
				int Start = (Step % Width == 0 ? 3 : 0);
				int End = ((Step + 1) % Width == 0 ? 3 : 0);

				for (int I = Start; I < DirLR.Length - End; I++) {
					int N = Step + DirLR[I];

					if ((N == From || (N >= 0 && N < Size && Passable[N] && GoodMoveLR(Step, I, Passable) && (Distance[N] > NextDistance)))) {
						Queue[Tail++] = N;
						Distance[N] = NextDistance + StepCost(N);
					} 
				}
			}

			return PathFound;
		}

		private static int BuildEscapeDistanceMap(int Cur, int From, float Factor, bool Passable) {
			System.Arraycopy(MaxVal, 0, Distance, 0, MaxVal.Length);
			int DestDist = Integer.MAX_VALUE;
			int Head = 0;
			int Tail = 0;
			Queue[Tail++] = From;
			Distance[From] = 0;
			int Dist = 0;

			while (Head < Tail) {
				int Step = Queue[Head++];
				Dist = Distance[Step];

				if (Dist > DestDist) {
					return DestDist;
				} 

				if (Step == Cur) {
					DestDist = (int) (Dist * Factor) + 1;
				} 

				int NextDistance = Dist + 1;
				int Start = (Step % Width == 0 ? 3 : 0);
				int End = ((Step + 1) % Width == 0 ? 3 : 0);

				for (int I = Start; I < DirLR.Length - End; I++) {
					int N = Step + DirLR[I];

					if (N >= 0 && N < Size && Passable[N] && GoodMoveLR(Step, I, Passable) && Distance[N] > NextDistance) {
						Queue[Tail++] = N;
						Distance[N] = NextDistance + StepCost(N);
						LastStep = N;
					} 
				}
			}

			return Dist;
		}

		public static Void BuildDistanceMap(int To, bool Passable) {
			System.Arraycopy(MaxVal, 0, Distance, 0, MaxVal.Length);
			int Head = 0;
			int Tail = 0;
			Queue[Tail++] = To;
			Distance[To] = 0;

			while (Head < Tail) {
				int Step = Queue[Head++];
				int NextDistance = Distance[Step] + 1;
				int Start = (Step % Width == 0 ? 3 : 0);
				int End = ((Step + 1) % Width == 0 ? 3 : 0);

				for (int I = Start; I < DirLR.Length - End; I++) {
					int N = Step + DirLR[I];

					if (N >= 0 && N < Size && Passable[N] && GoodMoveLR(Step, I, Passable) && (Distance[N] > NextDistance)) {
						Queue[Tail++] = N;
						Distance[N] = NextDistance + StepCost(N);
					} 
				}
			}
		}
	}
}
