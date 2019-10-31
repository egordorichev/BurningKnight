using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
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
			
			var callbacks = new DiscordRpc.EventHandlers();
			
			callbacks.disconnectedCallback += DisconnectedCallback;
			callbacks.errorCallback += ErrorCallback;
			callbacks.readyCallback += ReadyCallback;
			
			
			try {
				DiscordRpc.Initialize("459603244256198657", ref callbacks, true, string.Empty);
				UpdateStatus();
			} catch (Exception e) {
				Log.Error(e);	
			}
		}

		public override void Update(float dt) {
			base.Update(dt);
			lastUpdate += dt;

			if (lastUpdate >= 3f) {
				lastUpdate = 0;
				
				try {
					UpdateStatus();
					DiscordRpc.RunCallbacks();
				} catch (Exception e) {
					Log.Error(e);	
				}
			}
		}
		
		private void UpdateStatus() {
			var status = new DiscordRpc.RichPresence();

			if (Run.Level != null) {
				status.details = $"{Locale.Get(Run.Level.Biome.Id)} {(Run.Depth < 1 ? "" : MathUtils.ToRoman((Run.Depth - 1) % 2 + 1))}";
				var p = LocalPlayer.Locate(Engine.Instance.State.Area);

				if (p != null) {
					var h = p.GetComponent<HatComponent>().Item;

					if (h.Id != "bk:no_hat") {
						status.state = $"{(h?.Name ?? "No hat :(")}";
					}
				}
			}

			status.largeImageKey = "bk";
			status.largeImageText = "burningknight.net";
			status.startTimestamp = startTime;
			
			DiscordRpc.UpdatePresence(ref status);
		}

		public override void Destroy() {
			base.Destroy();
			DiscordRpc.Shutdown();
		}

		private static void ReadyCallback(ref DiscordRpc.DiscordUser user) {
			CurrentPlayer = user.username;
			Log.Info($"Discord connected! Welcome, {user.username}#{user.discriminator}!");
		}

		private static void DisconnectedCallback(int errorCode, string message) {
			CurrentPlayer = null;
			Log.Info($"Discord disconnected {errorCode} {message}");
		}

		private static void ErrorCallback(int errorCode, string message) {
			Log.Error($"Discord error {errorCode} {message}");
		}
	}
}
