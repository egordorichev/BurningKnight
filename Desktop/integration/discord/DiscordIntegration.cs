using System;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.state;
using Lens;
using Lens.assets;
using Lens.util;

namespace Desktop.integration.discord {
	public class DiscordIntegration : Integration {
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

			DiscordRpc.Initialize("459603244256198657", ref callbacks, true, string.Empty);

			UpdateStatus();
		}

		public override void Update(float dt) {
			base.Update(dt);
			lastUpdate += dt;

			if (lastUpdate >= 3f) {
				lastUpdate = 0;
				
				UpdateStatus();
			}
		}
		
		private void UpdateStatus() {
			var Status = new DiscordRpc.RichPresence();

			if (Run.Level != null) {
				Status.details = $"{Locale.Get(Run.Level.Biome.Id)} {(Run.Depth < 1 ? "" : MathUtils.ToRoman(Run.Depth))}";
				var p = LocalPlayer.Locate(Engine.Instance.State.Area);

				if (p != null) {
					var h = p.GetComponent<HatComponent>().Item;
					Status.state = $"{(h?.Name ?? "No hat :(")}";
				}
			}
			
			Status.startTimestamp = startTime;
			DiscordRpc.UpdatePresence(ref Status);
		}

		public override void Destroy() {
			base.Destroy();
			DiscordRpc.Shutdown();
		}

		private static void DisconnectedCallback(int errorCode, string message) {
			Console.WriteLine($"Discord::Disconnect({errorCode}, {message})");
		}

		private static void ErrorCallback(int errorCode, string message) {
			Console.WriteLine($"Discord::Error({errorCode}, {message})");
		}
	}
}
