namespace BurningKnight.state {
	public static class Run {
		private static int depth;
		public static int NextDepth { get; private set; }
		public static int LastDepth;
		
		public static int Depth {
			get => depth;
			set => NextDepth = value;
		}
				
		public static int KillCount;
		public static float Time;
		public static int Id;

		public static void UpdateLevel() {
			LastDepth = depth;
			depth = NextDepth;
		}
	}
}