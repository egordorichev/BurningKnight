using System;
using BurningKnight.state;
using Lens;
using Lens.util;
using TwitchLib.Client;
using TwitchLib.Client.Events;
using TwitchLib.Client.Models;
using TwitchLib.Communication.Clients;
using TwitchLib.Communication.Models;

namespace Desktop.integration.twitch {
	public class TwitchIntegration : Integration {
		private const string DevAccount = "egordorichev";
		
		public static string Boots = "rwzxul2y";
		private TwitchClient client;
		
		public override void Init() {
			base.Init();
			
			var credentials = new ConnectionCredentials("BurningKnightBot", $"{Pus}{DesktopApp.In}{Boots}");
			var customClient = new WebSocketClient(new ClientOptions {
				MessagesAllowedInPeriod = 750,
				ThrottlingPeriod = TimeSpan.FromSeconds(30)
			});
			
			client = new TwitchClient(customClient);
			client.Initialize(credentials, "egordorichev");

			client.OnConnected += OnConnected;
			client.OnMessageReceived += OnMessageReceived;
			
			client.Connect();
		}
		
		private static void OnConnected(object sender, OnConnectedArgs e) {
			Log.Info($"Connected to {e.AutoJoinChannel}");
		}

		private static void OnMessageReceived(object sender, OnMessageReceivedArgs e) {
			try {
				var state = Engine.Instance.State;

				if (!(state is InGameState gamestate)) {
					return;
				}

				var message = e.ChatMessage.Message;
				Log.Debug(message);

				if (e.ChatMessage.Username == DevAccount && message.StartsWith("sudo ")) {
					var command = message.Substring(5, message.Length - 5);
					Log.Debug(command);
					gamestate.Console.RunCommand(command);
				} else if (int.TryParse(message, out var number)) {
					Log.Info($"Voted for #{number}");
				}
			} catch (Exception ex) {
				Log.Error(ex);
			}
		}

		public override void Destroy() {
			client.Disconnect();
			base.Destroy();
		}
	}
}