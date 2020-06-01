using System;
using System.Collections.Generic;
using BurningKnight;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.twitch;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using Lens;
using Lens.util;
using Lens.util.math;
using Microsoft.Xna.Framework;
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
					client.SendMessage(channel, "The Knight is here, guys");
			
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

				client.OnNewSubscriber += (o, e) => {
					OnSub(e.Subscriber.DisplayName, e.Subscriber.ColorHex);
				};

				client.OnReSubscriber += (o, e) => {
					OnSub(e.ReSubscriber.DisplayName, e.ReSubscriber.ColorHex);
				};
				
				client.OnMessageReceived += OnMessageReceived;
				client.Connect();
			};

			TwitchBridge.TurnOff = (callback) => {
				Log.Info("Turning twitch integration off");
				
				controller = null;
				client.Disconnect();
				client = null;

				TwitchBridge.On = false;
				callback();
			};

			if (BK.Version.Dev) {
				TwitchBridge.TurnOn("egordorichev", (ok) => {
					
				});
			}
		}
		
		private List<TwitchPet> buffer = new List<TwitchPet>();

		private void OnSub(string who, string color) {
			Log.Info($"{who} subscribed!");
			
			buffer.Add(new TwitchPet {
				Nick = who,
				Color = color
			});
		}

		private void OnMessageReceived(object sender, OnMessageReceivedArgs e) {
			var dev = e.ChatMessage.Username == DevAccount;

			if (!dev && (Run.Depth < 1 || Run.Type != RunType.Twitch)) {
				return;
			}
			
			try {
				var state = Engine.Instance.State;

				if (!(state is InGameState gamestate)) {
					return;
				}

				var message = e.ChatMessage.Message;
				Log.Debug(message);

				if (dev) {
					if (message.StartsWith("sudo ")) {
						var command = message.Substring(5, message.Length - 5);

						switch (command) {
							case "msg": {
								enableDevMessages = !enableDevMessages;
								return;
							}
							
							case "sub": {
								OnSub("egordorichev", e.ChatMessage.ColorHex);
								return;
							}
						}
						
						Log.Debug(command);
						gamestate.Console.RunCommand(command);
						
						return;
					}
				}

				if (message.StartsWith("!")) {
					var m = message.Substring(1, message.Length - 1);
					var n = e.ChatMessage.DisplayName;

					if (m.StartsWith("color ")) {
						var pet = state.Area.Find<TwitchPet>(f => f is TwitchPet p && p.Nick == n);

						if (pet != null) {
							pet.Color = m.Substring(6, m.Length - 6);
							pet.UpdateColor();
							pet.GetComponent<AnimationComponent>().Animate();
						}
						
						return;
					} else if (m == "wink") {
						var pet = state.Area.Find<TwitchPet>(f => f is TwitchPet p && p.Nick == n);
						pet.GetComponent<AnimationComponent>().Animate();
						
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

			if (Run.Depth < 1 || Run.Type != RunType.Twitch || controller == null) {
				return;
			}
			
			controller.Update(dt);

			if (!(Engine.Instance.State is InGameState ingame)) {
				return;
			}

			if (buffer.Count > 0) {
				var player = LocalPlayer.Locate(ingame.Area);
				
				foreach (var p in buffer) {
					ingame.Area.Add(p);
					
					p.Center = player.Center + Rnd.Offset(24);
					AnimationUtil.Poof(p.Center, player.Depth + 1);
				}
				
				buffer.Clear();
			}
		}

		public void Render() {
			if (Run.Depth < 1 || Run.Type != RunType.Twitch) {
				return;
			}
			
			controller?.Render();
		}
	}
}