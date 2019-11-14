using System;
using BurningKnight.assets.achievements;
using BurningKnight.save;
using Lens.util;
using Steamworks;
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

				Log.Info("Starting from steam! <3");

				foreach (var achievement in SteamUserStats.Achievements) {
					Achievements.Unlock(achievement.Identifier);
				}

				Achievements.UnlockedCallback += (id) => {
					Log.Info($"Unlocking achievement {id} in steam!");
					new Achievement(id).Trigger();
				};
				
				SaveManager.LoadCloudSaves();
			} catch (Exception) {
				Log.Info("No steam no fire :/");
			}
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (LaunchedFromSteam) {
				SteamClient.RunCallbacks();
			}
		}

		public override void Destroy() {
			base.Destroy();

			if (LaunchedFromSteam) {
				SaveManager.SaveCloudSaves();
				SteamClient.Shutdown();
				
				LaunchedFromSteam = false;
			}
		}
	}
}