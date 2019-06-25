using System;
using BurningKnight.save;
using Lens.util;
using Steamworks;

namespace Desktop.integration.steam {
	public class SteamIntegration : Integration {
		public static bool LaunchedFromSteam { get; private set; }

		public override void Init() {
			base.Init();

			try {
				SteamClient.Init(4000);
				LaunchedFromSteam = true;
				SaveManager.EnableCloudSave = true;
				
				Log.Info("Starting from steam! <3");
			} catch (Exception e) {
				Log.Info("No steam no fire :/");
			}
		}

		public override void Destroy() {
			base.Destroy();

			if (LaunchedFromSteam) {
				SteamClient.Shutdown();
			}
		}
	}
}