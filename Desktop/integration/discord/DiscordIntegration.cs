using System;

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

			if (lastUpdate >= 5f) {
				UpdateStatus();
			}
		}
		
		private void UpdateStatus() {
			var Status = new DiscordRpc.RichPresence();
			
			Status.details = "Dying";
			Status.startTimestamp = startTime;
			Status.state = "state";

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
