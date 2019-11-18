using System;
using System.Runtime.InteropServices;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.level;
using BurningKnight.state;
using Lens;
using Lens.assets;
using Lens.util;

namespace Desktop.integration.discord {
	public class DiscordIntegration : Integration {
		public static string CurrentPlayer;
		
		private float lastUpdate;
		private long startTime;
		
		private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static long CurrentTimeMillis() {
			return (long) (DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
		}

		public override void Init() {
			base.Init();

			startTime = CurrentTimeMillis() / 1000;

			DiscordRpc.Initialize("459603244256198657", new DiscordRpc.EventHandlers());

			UpdateStatus();
		}

		public override void Update(float dt) {
			base.Update(dt);
			lastUpdate += dt;

			if (lastUpdate >= 3f) {
				lastUpdate = 0;
				
				UpdateStatus();
				DiscordRpc.RunCallbacks();
			}
		}
		
		private void UpdateStatus() {
			if (Run.Level?.Biome == null) {
				return;
			}
			
			var status = new DiscordRpc.RichPresence();

			if (Run.Level != null) {
				status.details = $"{Locale.Get(Run.Level.Biome.Id)} {Level.GetDepthString()}";
				var p = LocalPlayer.Locate(Engine.Instance.State.Area);

				if (p != null) {
					var h = p.GetComponent<HatComponent>().Item;

					if (h != null && h.Id != "bk:no_hat") {
						status.state = $"{(h?.Name ?? "No hat :(")}";
					}
				}
			}

			status.largeImageKey = "bk";
			status.largeImageText = "burningknight.net";
			status.startTimestamp = startTime;
			
			DiscordRpc.UpdatePresence(status);
		}

		public override void Destroy() {
			base.Destroy();
			DiscordRpc.Shutdown();
		}
	}
}
