using System;
using BurningKnight;
using BurningKnight.state;
using BurningKnight.ui.dialog;
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
		private bool enableDevMessages = true;
		
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
		
		private void OnConnected(object sender, OnConnectedArgs e) {
			Log.Info($"Connected to {e.AutoJoinChannel}");
		}

		private void OnMessageReceived(object sender, OnMessageReceivedArgs e) {
			try {
				var state = Engine.Instance.State;

				if (!(state is InGameState gamestate)) {
					return;
				}

				var message = e.ChatMessage.Message;
				Log.Debug(message);

				var dev = e.ChatMessage.Username == DevAccount;

				if (dev) {
					if (message == "sudo msg") {
						enableDevMessages = !enableDevMessages;
					} else if (message.StartsWith("sudo ")) {
						var command = message.Substring(5, message.Length - 5);
						Log.Debug(command);
						gamestate.Console.RunCommand(command);
					} else if (enableDevMessages) {
						var a = state.Area.Tagged[Tags.BurningKnight];

						if (a.Count > 0) {
							var bk = a[0];
							bk.GetComponent<DialogComponent>().StartAndClose(message, 3);
						}
					}
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