using DiscordRPC;
using DiscordRPC.Logging;
using Lens.util;

namespace BurningKnight.integration.discord {
	public static class Discord {
		private static DiscordRpcClient client;
		
		public static void Init() {
			/*client = new DiscordRpcClient("459603244256198657");
			client.Logger = new ConsoleLogger {
				Level = LogLevel.Info
			};

			client.OnReady += (sender, e) => {
				Log.Info($"Discord RPC loaded, welcome {e.User.Username}!");
			};

			client.Initialize();
			
			client.SetPresence(new RichPresence {
				Details = "Burning Knight",
				State = "Trying to work",
				
				Assets = new Assets {
					LargeImageKey = "hero_mercy",
					LargeImageText = "DISCORD RPC IS WOOORKIGN",
					SmallImageKey = "hero_mercy"
				}
			});*/	
		}

		public static void Destroy() {
			// client.Dispose();			
		}
	}
}