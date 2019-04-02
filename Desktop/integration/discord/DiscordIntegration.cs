using Desktop.integration.discord.NamedPipes;
using DiscordRPC;
using Lens.util;

namespace Desktop.integration.discord {
	public class DiscordIntegration : Integration {
		private DiscordRpcClient client;
		
		public override void Init() {
			base.Init();
			
			client = new DiscordRpcClient("459603244256198657", -1, null, true, new UnityNamedPipe());
            
			client.OnReady += (sender, msg) => Log.Info($"Connected to discord with user {msg.User.Username}");
			client.OnPresenceUpdate += (sender, msg) => Log.Debug("Presence has been updated!");
			client.OnError += (s, args) => Log.Error("[DRP] Error Occured within the Discord IPC: (" + args.Code + ") " + args.Message);
			client.OnConnectionFailed += (sender, msg) => Log.Error("Connected fail " + msg.FailedPipe);
			client.OnConnectionEstablished += (sender, msg) => { Log.Info("Connected ok"); };
			
			client.Initialize();

			client.SetPresence(new RichPresence {
				Details = "A test",
				State = "Trying to work",

				Assets = new Assets {
					LargeImageKey = "hero_mercy",
					LargeImageText = "It's woorking!",
					SmallImageKey = "hero_mercy"
				}
			});
		}

		public override void Update(float dt) {
			client?.Invoke();
		}

		public override void Destroy() {
			base.Destroy();
			client?.Dispose();
		}
	}
}
