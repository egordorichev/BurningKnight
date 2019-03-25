using BurningKnight.entity.level;

namespace BurningKnight.state {
	public static class Run {
		private static int depth;
		public static int NextDepth { get; private set; }
		
		public static int Depth {
			get => depth;
			set => NextDepth = value;
		}
				
		public static int KillCount;
		public static float Time;
		public static int Id = -1;

		public static void UpdateLevel() {
			depth = NextDepth;
		}

		public static Level Level;
	}
}