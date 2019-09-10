using System;
using System.Collections.Generic;
using System.IO;
using BurningKnight.save;
using Lens.input;
using Lens.lightJson;
using Lens.lightJson.Serialization;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.assets.input {
	public static class Controls {
		private static List<Control> controls = new List<Control>();
		private static List<Control> custom = new List<Control>();

		public const string Up = "up";
		public const string Left = "left";
		public const string Down = "down";
		public const string Right = "right";
		
		public const string Active = "active";
		public const string Use = "use";
		public const string Bomb = "bomb";
		public const string Interact = "interact";
		public const string Swap = "swap";

		public const string Roll = "roll";
		public const string Duck = "duck";
		
		public const string Pause = "pause";

		public const string UiAccept = "ui_accept";
		public const string GameStart = "game_start";
		public const string Cancel = "cancel";
		
		public const string Fullscreen = "fullscreen";
		public const string Mute = "mute";

		static Controls() {
			controls.Clear();
			
			controls.Add(new Control(Up, Keys.W, Keys.Up));
			controls.Add(new Control(Left, Keys.A, Keys.Left));
			controls.Add(new Control(Down, Keys.S, Keys.Down));
			controls.Add(new Control(Right, Keys.D, Keys.Right));

			controls.Add(new Control(Active, Keys.Space).Gamepad(Buttons.B));
			controls.Add(new Control(Use).Mouse(MouseButtons.Left).Gamepad(Buttons.RightTrigger));

			controls.Add(new Control(Bomb, Keys.Q).Gamepad(Buttons.Y));
			controls.Add(new Control(Interact, Keys.E).Gamepad(Buttons.A));
			controls.Add(new Control(Swap, Keys.LeftShift).Gamepad(Buttons.X));
			controls.Add(new Control(Roll).Mouse(MouseButtons.Right).Gamepad(Buttons.LeftTrigger));
			controls.Add(new Control(Duck, Keys.R).Gamepad(Buttons.LeftShoulder));

			controls.Add(new Control(Pause, Keys.Escape).Gamepad(Buttons.Back));
			controls.Add(new Control(UiAccept).Mouse(MouseButtons.Left));
			
			controls.Add(new Control(Mute, Keys.M));
			controls.Add(new Control(Fullscreen, Keys.F11, Keys.F));
			
			controls.Add(new Control(Cancel, Keys.Escape).Gamepad(Buttons.Back));
			controls.Add(new Control(GameStart, Keys.Space, Keys.Enter, Keys.X).Gamepad(Buttons.X, Buttons.Start));
		}

		public static void Bind() {
			Bind(custom);
		}
		
		public static void BindDefault() {
			Bind(controls);
		}

		private static void Bind(List<Control> controls) {
			Input.ClearBindings();

			foreach (var c in controls) {
				if (c.Keys != null) {
					Input.Bind(c.Id, c.Keys);
				}

				if (c.Buttons != null) {
					Input.Bind(c.Id, c.Buttons);
				}

				if (c.MouseButtons != null) {
					Input.Bind(c.Id, c.MouseButtons);
				}
			}
		}

		public static void Save() {
			try {
				var p = new FileHandle($"{SaveManager.SaveDir}keybindings.json").FullPath;
				Log.Info($"Saving keybindings to {p}");

				var file = File.CreateText(p);
				var writer = new JsonWriter(file, true);
				var root = new JsonObject();

				foreach (var t in (custom.Count == 0 ? controls : custom)) {
					var o = new JsonObject();

					if (t.Keys != null) {
						var a = new JsonArray();

						foreach (var k in t.Keys) {
							a.Add(k.ToString());
						}
						
						o["keys"] = a;
					}

					if (t.MouseButtons != null) {
						var a = new JsonArray();

						foreach (var k in t.MouseButtons) {
							a.Add(k.ToString());
						}
						
						o["mouse"] = a;
					}

					if (t.Buttons != null) {
						var a = new JsonArray();

						foreach (var k in t.Buttons) {
							a.Add(k.ToString());
						}
						
						o["gamepad"] = a;
					}
					
					root[t.Id] = o;
				}

				writer.Write(root);
				file.Close();
			} catch (Exception e) {
				Log.Error(e);
			}
		}

		public static void Load() {
			try {
				var handle = new FileHandle($"{SaveManager.SaveDir}keybindings.json");

				if (!handle.Exists()) {
					Log.Info("Keybindings file was not found, creating new one");

					BindDefault();
					Save();

					return;
				}
				
				Log.Info("Loading keybindings");

				var root = JsonValue.Parse(handle.ReadAll());
				custom.Clear();

				foreach (var pair in root.AsJsonObject) {
					var control = new Control(pair.Key);
					
					if (pair.Value["keys"].IsJsonArray) {
						var l = new List<Keys>();
						
						foreach (var k in pair.Value["keys"].AsJsonArray) {
							if (Enum.TryParse<Keys>(k.String(""), out var key)) {
								l.Add(key);
							}
						}

						control.Keys = l.ToArray();
					}
					
					if (pair.Value["mouse"].IsJsonArray) {
						var l = new List<MouseButtons>();
						
						foreach (var k in pair.Value["mouse"].AsJsonArray) {
							if (Enum.TryParse<MouseButtons>(k.String(""), out var key)) {
								l.Add(key);
							}
						}

						control.MouseButtons = l.ToArray();
					}
					
					if (pair.Value["gamepad"].IsJsonArray) {
						var l = new List<Buttons>();
						
						foreach (var k in pair.Value["gamepad"].AsJsonArray) {
							if (Enum.TryParse<Buttons>(k.String(""), out var key)) {
								l.Add(key);
							}
						}

						control.Buttons = l.ToArray();
					}
					
					custom.Add(control);
				}

				Bind();
			} catch (Exception e) {
				Log.Error(e);
			}
		}
	}
}