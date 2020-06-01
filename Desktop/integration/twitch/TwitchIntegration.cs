using System;
using BurningKnight;
using BurningKnight.entity.twitch;
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
		private static string Boots = "rwzxul2y";
		
		private TwitchClient client;
		private TwitchContoller controller;
		private bool enableDevMessages = true;
		
		public override void Init() {
			base.Init();

			TwitchBridge.TurnOn = (channel, callback) => {
				var credentials = new ConnectionCredentials("BurningKnightBot", $"{Pus}{DesktopApp.In}{Boots}");
				var customClient = new WebSocketClient(new ClientOptions {
					MessagesAllowedInPeriod = 750,
					ThrottlingPeriod = TimeSpan.FromSeconds(30)
				});
			
				client = new TwitchClient(customClient);
				client.Initialize(credentials, channel);

				client.OnConnected += (o, e) => {
					TwitchBridge.LastUsername = channel;

					Log.Info($"Connected to {e.AutoJoinChannel}");
					callback(true);
					client.SendMessage(channel, "Heyo, pogs");
			
					controller = new TwitchContoller();
					controller.Init();

					TwitchBridge.On = true;
				};

				client.OnError += (sender, args) => {
					Log.Error(args.Exception.Message);
				};
				
				client.OnConnectionError += (o, e) => {
					callback(false);
				};
				
				client.OnMessageReceived += OnMessageReceived;
			
				client.Connect();
			};

			TwitchBridge.TurnOff = (callback) => {
				controller = null;
				client.Disconnect();
				client = null;

				TwitchBridge.On = false;
				callback();
			};
		}

		private void OnMessageReceived(object sender, OnMessageReceivedArgs e) {
			if (Run.Depth < 1 || Run.Type != RunType.Twitch) {
				return;
			}
			
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
						return;
					} 
					
					if (message.StartsWith("sudo ")) {
						var command = message.Substring(5, message.Length - 5);
						Log.Debug(command);
						gamestate.Console.RunCommand(command);
						
						return;
					}
				}

				if (controller != null && !controller.HandleMessage(e.ChatMessage) && dev) {
					if (enableDevMessages) {
						var a = state.Area.Tagged[Tags.BurningKnight];

						if (a.Count > 0) {
							var bk = a[0];
							bk.GetComponent<DialogComponent>().StartAndClose(message, 5);
						}
					}
				}
			} catch (Exception ex) {
				Log.Error(ex);
			}
		}

		public override void Destroy() {
			client?.Disconnect();
			base.Destroy();
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			if (Run.Depth < 1 || Run.Type != RunType.Twitch) {
				return;
			}
			
			controller?.Update(dt);
		}

		public void Render() {
			if (Run.Depth < 1 || Run.Type != RunType.Twitch) {
				return;
			}
			
			controller?.Render();
		}
	}
}