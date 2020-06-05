using System;
using System.Collections.Generic;
using BurningKnight;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.twitch;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using BurningKnight.util;
using Lens;
using Lens.assets;
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
		public const string DevAccount = "egordorichev";
		private static string Boots = "rwzxul2y";
		
		private TwitchClient client;
		private TwitchContoller controller;
		private bool enableDevMessages = true;
		private bool spawnAllPets;
		
		public override void Init() {
			base.Init();

			TwitchBridge.TurnOn = (channel, callback) => {
				if (TwitchBridge.On) {
					return;
				}

				TwitchBridge.On = true;
				
				try {
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
						client.SendMessage(channel, "The Knight is here, bois");
				
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

					client.OnGiftedSubscription += (o, e) => {
						OnSub(e.GiftedSubscription.DisplayName, "#ff00ff");
					};
					
					client.OnMessageReceived += OnMessageReceived;
					client.Connect();

					TwitchBridge.On = true;
				} catch (Exception e) {
					TwitchBridge.On = false;
					Log.Error(e);
				}
			};

			TwitchBridge.TurnOff = (callback) => {
				Log.Info("Turning twitch integration off");
				
				controller = null;
				client.Disconnect();
				client = null;

				TwitchBridge.On = false;
				callback();
			};

			TwitchBridge.OnHubEnter += () => {
				spawnAllPets = true;
			};

			TwitchBridge.OnNewRun += () => {
				if (Run.Type == RunType.Twitch) {
					spawnAllPets = true;
				}
			};
		}

		public override void PostInit() {
			base.PostInit();
			var id = GlobalSave.GetString("twitch_username");

			if (id == null) {
				return;
			}
			
			TwitchBridge.TurnOn(id, (ok) => {
					
			});
		}

		private class Data {
			public string Nick;
			public string Color;
		}
		
		private List<Data> buffer = new List<Data>();
		private List<Data> totalBuffer = new List<Data>();
		private List<string> messageIds = new List<string>();

		private void OnSub(string who, string color) {
			// Just to be safe from threading tbh
			for (var i = 0; i < buffer.Count; i++) {
				if (buffer[i].Nick == who) {
					return;
				}
			}
			
			for (var i = 0; i < totalBuffer.Count; i++) {
				if (totalBuffer[i].Nick == who) {
					return;
				}
			}
			
			Log.Info($"{who} subscribed!");
			Audio.PlaySfx("level_cleared");
			
			buffer.Add(new Data {
				Nick = who,
				Color = color
			});
			
			totalBuffer.Add(new Data {
				Nick = who,
				Color = color
			});
		}

		private void OnMessageReceived(object sender, OnMessageReceivedArgs e) {
			var dev = e.ChatMessage.Username == DevAccount;

			if (!dev && (Run.Depth < 1 || Run.Type != RunType.Twitch)) {
				return;
			}
			

			if (messageIds.Contains(e.ChatMessage.Id)) {
				return;
			}

			messageIds.Add(e.ChatMessage.Id);
			
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
							var cl = m.Substring(6, m.Length - 6);
							
							pet.Color = cl;
							pet.UpdateColor();
							pet.GetComponent<AnimationComponent>().Animate();

							foreach (var p in totalBuffer) {
								if (p.Nick == pet.Nick) {
									p.Color = cl;
									break;
								}
							}
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
			try {
				client?.Disconnect();
			} catch (Exception e) {
				Log.Error(e);
			}
			
			base.Destroy();
		}

		private float t;

		public override void Update(float dt) {
			base.Update(dt);

			t += dt;

			if (t >= 10f) {
				t = 0;
				messageIds.Clear();
			}

			try {
				if (controller != null && Run.Type == RunType.Twitch && Run.Depth > 0) {
					controller.Update(dt);
				}
				
				if (!(Engine.Instance.State is InGameState ingame)) {
					return;
				}

				if (spawnAllPets && (Run.Type == RunType.Twitch || Run.Depth == 0)) {
					SpawnPets(totalBuffer);
					spawnAllPets = false;
				}
				
				if (buffer.Count > 0) {
					SpawnPets(buffer);
					buffer.Clear();
				}
			} catch (Exception e) {
				Log.Error(e);
			}
		}
		
		private void SpawnPets(List<Data> pets) {
			var ingame = (InGameState) Engine.Instance.State;
			var player = LocalPlayer.Locate(ingame.Area);
				
			foreach (var d in pets) {
				var p = new TwitchPet {
					Nick = d.Nick,
					Color = d.Color
				};
					
				ingame.Area.Add(p);
					
				p.Center = player.Center + Rnd.Offset(24);
				AnimationUtil.Poof(p.Center, player.Depth + 1);
				
				AnimationUtil.Confetti(p.Center);
			}
		}

		public void Render() {
			if (Run.Depth < 1 || Run.Type != RunType.Twitch) {
				return;
			}

			try {
				controller?.Render();
			} catch (Exception e) {
				Log.Error(e);
			}
		}
	}
}