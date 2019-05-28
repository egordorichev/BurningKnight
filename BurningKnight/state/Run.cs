using System;
using BurningKnight.level;
using BurningKnight.save;
using Lens;

namespace BurningKnight.state {
	public static class Run {
		private static int depth = 0;
		public static int NextDepth { get; private set; } = depth;
		public static int LastDepth = depth;
		public static bool StartingNew;
		public static int KillCount;
		public static float Time;
		public static Level Level;
		public static bool StartedNew;
		public static bool HasRun;

		public static int Depth {
			get => depth;
			set => NextDepth = value;
		}

		public static void SetDepth(int value) {
			depth = value;
		}

		public static void Update() {
			if (StartingNew) {
				NextDepth = 1;
				SaveManager.Delete(SaveType.Game, SaveType.Level, SaveType.Player);
			}

			if (StartingNew || depth != NextDepth) {
				LastDepth = depth;
				depth = NextDepth;
				StartedNew = StartingNew;
				StartingNew = false;
				Engine.Instance.SetState(new LoadState());
			}
		}

		public static void StartNew() {
			StartingNew = true;
			HasRun = false;
		}

		public static void ResetStats() {
			KillCount = 0;
			Time = 0;
			HasRun = false;
		}

		public static string FormatTime() {
			return $"{Math.Floor(Time / 360f)}h {Math.Floor(Time / 60f)}m {Math.Floor(Time)}s";
		}
	}
}