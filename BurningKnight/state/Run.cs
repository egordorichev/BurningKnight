using System;
using BurningKnight.level;
using BurningKnight.save;
using Lens;

namespace BurningKnight.state {
	public static class Run {
		private static int depth = 1;
		public static int NextDepth { get; private set; } = depth;
		public static int LastDepth = depth;
		public static bool StartingNew;
		public static int KillCount;
		public static float Time;
		public static int Id = 0;
		public static Level Level;

		public static int Depth {
			get => depth;
			set => NextDepth = value;
		}

		public static void SetDepth(int value) {
			depth = value;
		}

		public static void Update() {
			if (depth != NextDepth) {
				LastDepth = depth;
				depth = NextDepth;
				Engine.Instance.SetState(new LoadState());
			}
		}

		public static void StartNew() {
			LastDepth = depth;
			NextDepth = 1;
			
			StartingNew = true;
			SaveManager.Delete(SaveType.Game, SaveType.Level, SaveType.Player);
			
			Engine.Instance.SetState(new LoadState());
		}

		public static void ResetStats() {
			KillCount = 0;
			Time = 0;
			Id++;
		}

		public static string FormatTime() {
			return $"{Math.Floor(Time / 360f)}h {Math.Floor(Time / 60f)}m {Math.Floor(Time)}s";
		}
	}
}