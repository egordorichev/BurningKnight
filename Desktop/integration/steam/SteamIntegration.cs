using System;
using System.Threading;
using BurningKnight.assets.achievements;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui;
using Lens;
using Lens.assets;
using Lens.util;
using Steamworks;
using Steamworks.Data;
using Achievement = Steamworks.Data.Achievement;

namespace Desktop.integration.steam {
	public class SteamIntegration : Integration {
		public static bool LaunchedFromSteam { get; private set; }

		public override void Init() {
			base.Init();

			try {
				SteamClient.Init(851150);

				LaunchedFromSteam = true;
				SaveManager.EnableCloudSave = true;
				var lang = SteamApps.GameLanguage.ToLower();

				Log.Info($"Starting from steam! <3 ({lang})");

				switch (lang) {
					case "english": {
						Locale.PrefferedClientLanguage = "en";
						break;
					}
					
					case "russian": {
						Locale.PrefferedClientLanguage = "ru";
						break;
					}
					
					case "german": {
						Locale.PrefferedClientLanguage = "de";
						break;
					}
					
					case "italian": {
						Locale.PrefferedClientLanguage = "it";
						break;
					}
					
					case "polish": {
						Locale.PrefferedClientLanguage = "pl";
						break;
					}
					
					case "french": {
						Locale.PrefferedClientLanguage = "fr";
						break;
					}
				}

				Run.SubmitScore += (score, board) => {
					try {
						new Thread(() => {
							try {
								Log.Info($"Submitting score {score} to board {board}");

								var br = SteamUserStats
									.FindOrCreateLeaderboardAsync(board, LeaderboardSort.Descending, LeaderboardDisplay.Numeric)
									.GetAwaiter().GetResult();

								br?.SubmitScoreAsync(score).GetAwaiter().GetResult();

								Log.Info($"Done submitting the score {score}");
							} catch (Exception e) {
								Log.Error(e);
							}
						}).Start();
					} catch (Exception e) {
						Log.Error(e);
					}
				};

				InGameState.SetupLeaderboard += (stats, boardId, type, offset, end) => {
					try {
						new Thread(() => {
							try {
								var i = 0;
								var count = 0;
								var name = SteamClient.Name;

								var board = SteamUserStats
									.FindOrCreateLeaderboardAsync(boardId, LeaderboardSort.Descending, LeaderboardDisplay.Numeric)
									.GetAwaiter().GetResult().Value;

								LeaderboardEntry[] scores;

								if (type == "global") {
									scores = board.GetScoresAsync(10, Math.Max(1, offset)).GetAwaiter().GetResult();
								} else if (type == "friends") {
									scores = board.GetScoresFromFriendsAsync().GetAwaiter().GetResult();
									i = Math.Max(0, offset);
								} else {
									scores = board.GetScoresAroundUserAsync(-5 + offset, 5 + offset).GetAwaiter().GetResult();
								}

								if (scores != null) {
									for (; i < scores.Length && count < 10; i++) {
										var score = scores[i];

										var n = score.User.Name;
										n = n.Substring(0, Math.Min(n.Length, 18));

										stats.Add($"#{score.GlobalRank} {n}", score.Score.ToString(), score.User.Name == name);

										count++;
									}
								} else {
									stats.Add(Locale.Get("no_scores_yet"), ":(");
								}

								end();
							} catch (Exception e) {
								Log.Error(e);
							}
						}).Start();
					} catch (Exception e) {
						Log.Error(e);
					}
				};

				Achievements.PostLoadCallback += () => {
					try {
						foreach (var achievement in SteamUserStats.Achievements) {
							if (achievement.State) {
								Achievements.Unlock(achievement.Identifier);
							}
						}

						foreach (var achievement in Achievements.Defined) {
							if (achievement.Value.Unlocked) {
								new Achievement(achievement.Key).Trigger();
							}
						}
					} catch (Exception e) {
						Log.Error(e);
					}
				};

				Achievements.UnlockedCallback += (id) => {
					try {
						new Achievement(id).Trigger();
					} catch (Exception e) {
						Log.Error(e);
					}
				};

				Achievements.LockedCallback += (id) => {
					try {
						new Achievement(id).Clear();
					} catch (Exception e) {
						Log.Error(e);
					}
				};

				Achievements.ProgressSetCallback += (id, progress, max) => {
					try {
						SteamUserStats.IndicateAchievementProgress(id, progress, max);
					} catch (Exception e) {
						Log.Error(e);
					}
				};

				try {
					SaveManager.LoadCloudSaves();
				} catch (Exception e) {
					Log.Error(e);
				}

				SteamFriends.OnGameOverlayActivated += () => {
					Engine.Instance.State.Paused = true;
				};
				
			} catch (Exception e) {
				Log.Error(e);
				Log.Info("No steam no fire :/");
			}
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (LaunchedFromSteam) {
				try {
					SteamClient.RunCallbacks();
				} catch (Exception e) {
					Log.Error(e);
				}
			}
		}

		public override void Destroy() {
			base.Destroy();

			if (LaunchedFromSteam) {
				try {
					SaveManager.SaveCloudSaves();
					SteamClient.Shutdown();
				} catch (Exception e) {
					Log.Error(e);
				}

				LaunchedFromSteam = false;
			}
		}
	}
}