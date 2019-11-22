using System;
using BurningKnight.level;
using BurningKnight.save;
using BurningKnight.save.statistics;
using Lens;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.state {
	public static class Run {
		public const int ContentEndDepth = 5;

		private static int depth = BK.Version.Dev ? 4 : 0;
		public static int NextDepth = depth;
		public static int LastDepth = depth;
		public static int SavingDepth;
		public static bool StartingNew;
		public static int KillCount;
		public static float Time;
		public static Level Level;
		public static bool StartedNew;
		public static bool HasRun;
		public static string Seed;
		public static bool IgnoreSeed;
		public static int Luck;
		public static int Curse { get; private set; }
		public static bool IntoMenu;
		public static RunStatistics Statistics;
		public static string NextSeed;
		public static int LastSavedDepth;
		
		public static int Depth {
			get => depth;
			set => NextDepth = value;
		}

		public static void Update() {
			if (StartingNew || depth != NextDepth) {
				LastDepth = depth;
				SavingDepth = depth;
				depth = NextDepth;
				StartedNew = StartingNew;

				if (StartingNew) {
					SaveManager.Delete(SaveType.Player, SaveType.Game, SaveType.Level);
				}
				
				StartingNew = false;

				Engine.Instance.SetState(new LoadState {
					Menu = IntoMenu
				});

				IntoMenu = false;
			}
		}

		public static void StartNew(int depth = 1) {
			StartingNew = true;
			HasRun = false;
			NextDepth = depth;

			if (NextSeed != null) {
				Seed = NextSeed;
				NextSeed = null;
				IgnoreSeed = false;
			} else if (IgnoreSeed) {
				IgnoreSeed = false;
			} else {
				Seed = Rnd.GenerateSeed();
			}

			GlobalSave.RunId++;
			Rnd.Seed = Seed;
			
			Log.Debug($"This run's seed is {Seed}");
		}

		public static void ResetStats() {
			KillCount = 0;
			Time = 0;
			HasRun = false;
			Luck = 0;
			Curse = 0;			
			LastSavedDepth = 0;
		}

		public static string FormatTime() {
			return $"{Math.Floor(Time / 3600f)}h {Math.Floor(Time / 60f)}m {Math.Floor(Time % 60f)}s";
		}

		public static void AddCurse() {
			Curse++;
		}

		public static void ResetCurse() {
			Curse = 0;
		}
	}
}