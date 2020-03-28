using System;
using BurningKnight.assets.achievements;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.level;
using BurningKnight.save;
using BurningKnight.save.statistics;
using Lens;
using Lens.assets;
using Lens.util;
using Lens.util.math;
using Steamworks.Data;

namespace BurningKnight.state {
	public static class Run {
		public static Action<int, string> SubmitScore;
		public static int ContentEndDepth = BK.Demo ? 3 : 11;

		private static int depth = BK.Version.Dev ? 2 : 0;
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
		public static int Scourge { get; private set; }
		public static int PermanentScourge { get; internal set; }
		public static bool IntoMenu;
		public static RunStatistics Statistics;
		public static string NextSeed;
		public static int LastSavedDepth;
		public static bool AlternateMusic;
		public static RunType Type;
		public static int Score;
		public static int DailyId;
		public static byte ChallengeId;
		public static bool Won;
		
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

		public static void StartNew(int depth = 1, RunType type = RunType.Regular) {
			StartingNew = true;
			HasRun = false;
			NextDepth = depth;
			Type = type;

			if (NextSeed != null) {
				Seed = NextSeed;
				NextSeed = null;
				IgnoreSeed = false;
			} else if (IgnoreSeed) {
				IgnoreSeed = false;
			} else {
				Seed = Rnd.GenerateSeed();
			}

			if (Type != RunType.Challenge) {
				ChallengeId = 0;
			} else {
				Log.Info($"Starting challenge {ChallengeId}");
			}

			if (Type == RunType.Daily) {
				var date = DateTime.UtcNow;
				DailyId = (date.Year - 2020) * 365 + (date.DayOfYear);
				Log.Debug($"Today is {date.DayOfYear} day of the year {date.Year}, so the daily id is {DailyId}");

				Seed = Rnd.GenerateSeed(8, DailyId);
			} else {
				DailyId = 0;
			}

			GlobalSave.RunId++;
			Rnd.Seed = Seed;
			AlternateMusic = Rnd.Chance(0.5f);
			
			Log.Debug($"This run's seed is {Seed}");
		}

		public static void ResetStats() {
			KillCount = 0;
			Time = 0;
			HasRun = false;
			Luck = 0;
			Scourge = 0;
			Won = false;
			PermanentScourge = 0;
			LastSavedDepth = 0;
			
			entity.item.Scourge.Clear();
		}

		public static string FormatTime() {
			return $"{Math.Floor(Time / 3600f)}h {Math.Floor(Time / 60f)}m {Math.Floor(Time % 60f)}s";
		}

		public static void RemoveScourge() {
			if (Scourge == 0) {
				return;
			}
			
			PermanentScourge = Math.Max(0, PermanentScourge - 1);
			Scourge--;
			
			var player = LocalPlayer.Locate(Engine.Instance.State.Area);

			if (player == null) {
				return;
			}
			
			TextParticle.Add(player, Locale.Get("scourge"), 1, true, true);
		}

		public static void AddScourge(bool permanent = false) {
			Scourge++;
			Audio.PlaySfx("player_cursed");

			if (Scourge >= 10) {
				Achievements.Unlock("bk:scourge_king");
			}

			if (Scourge > 10) {
				Scourge = 10;
			}
			
			var player = LocalPlayer.Locate(Engine.Instance.State.Area);

			if (player == null) {
				return;
			}
			
			TextParticle.Add(player, Locale.Get("scourge"), 1, true);
			var center = player.Center;
			
			for (var i = 0; i < 10; i++) {
				var part = new ParticleEntity(Particles.Scourge());
						
				part.Position = center + Rnd.Vector(-4, 4);
				part.Particle.Scale = Rnd.Float(0.4f, 0.8f);
				Level.Area.Add(part);
				part.Depth = 1;
			}
			
			if (permanent) {
				PermanentScourge++;

				if (PermanentScourge > 10) {
					PermanentScourge = 10;
				}
			}
		}

		public static void ResetScourge() {
			Scourge = PermanentScourge;
		}

		public static void CalculateScore() {
			Score = 0;

			Score += Depth * 5000;
			Score += Statistics.CoinsObtained * 10;
			Score += Statistics.Items.Count * 100;
			Score += (int) Statistics.MobsKilled * 10;
			Score += (int) Statistics.RoomsExplored * 2;
			Score += Statistics.BossesDefeated * 1000;

			if (Won) {
				Score += 5000;
			}
			
			Score -= (int) Statistics.DamageTaken * 10;
			Score -= (int) Time;
			Score -= Statistics.PitsFallen;
		}

		public static void Win() {
			if (Won) {
				return;
			}

			Won = true;
			Statistics.Won = true;

			foreach (var p in Engine.Instance.State.Area.Tagged[Tags.Player]) {
				p.RemoveComponent<PlayerInputComponent>();
				p.GetComponent<HealthComponent>().Unhittable = true;
			}
			
			((InGameState) Engine.Instance.State).AnimateDoneScreen();
		}
	}
}