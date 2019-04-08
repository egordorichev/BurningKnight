using BurningKnight.entity.level;
using Lens;

namespace BurningKnight.state {
	public static class Run {
		private static int depth = -1;
		public static int NextDepth { get; private set; } = depth;
		public static int LastDepth;
		
		public static int Depth {
			get => depth;
			set => NextDepth = value;
		}
				
		public static int KillCount;
		public static float Time;
		public static int Id = -1;

		public static void Update() {
			if (depth != NextDepth) {
				LastDepth = depth;
				depth = NextDepth;
				Engine.Instance.SetState(new LoadState());
			}			
		}

		public static Level Level;
	}
}