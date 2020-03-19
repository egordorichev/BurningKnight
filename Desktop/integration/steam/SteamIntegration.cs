using System;
using System.Threading;
using BurningKnight.assets.achievements;
using BurningKnight.save;
using BurningKnight.state;
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

				/*var board = SteamUserStats.FindLeaderboardAsync("daily").GetAwaiter().GetResult().Value;
				var results = board.GetScoresAsync(10).GetAwaiter().GetResult();

				if (results != null) {
					foreach (var r in results) {
						Log.Debug($"#{r.GlobalRank} @{r.User.Name} = {r.Score}");
					}
				} else {
					Log.Debug("No results");
				}

				board.SubmitScoreAsync(1).GetAwaiter().GetResult();*/
				
				LaunchedFromSteam = true;
				SaveManager.EnableCloudSave = true;

				Log.Info("Starting from steam! <3");

				Run.SubmitScore += (score, board) => {
					new Thread(() => { 
						Log.Info($"Submitting score {score} to board {board}");
					
						var br = SteamUserStats.FindOrCreateLeaderboardAsync(board, LeaderboardSort.Descending, LeaderboardDisplay.Numeric).GetAwaiter().GetResult();
						br?.SubmitScoreAsync(score).GetAwaiter().GetResult();

						Log.Info($"Done submitting the score {score}");
					}).Start();
				};

				Achievements.PostLoadCallback += () => {
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
				};

				Achievements.UnlockedCallback += (id) => {
					new Achievement(id).Trigger();
				};

				Achievements.LockedCallback += (id) => {
					new Achievement(id).Clear();
				};

				try {
					SaveManager.LoadCloudSaves();
				} catch (Exception e) {
					Log.Error(e);
				}
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