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
		public const string Map = "map";
		
		public const string Pause = "pause";

		public const string GameStart = "game_start";
		public const string Cancel = "cancel";
		
		public const string Fullscreen = "fullscreen";
		public const string Fps = "fps";

		public const string UiUp = "ui_up";
		public const string UiDown = "ui_down";
		public const string UiLeft = "ui_left";
		public const string UiRight = "ui_right";
		public const string UiAccept = "ui_accept";
		public const string UiSelect = "ui_select";
		public const string UiBack = "ui_back";

		static Controls() {
			controls.Clear();
			
			controls.Add(new Control(Up, Keys.W, Keys.Up));
			controls.Add(new Control(Left, Keys.A, Keys.Left));
			controls.Add(new Control(Down, Keys.S, Keys.Down));
			controls.Add(new Control(Right, Keys.D, Keys.Right));

			controls.Add(new Control(Active, Keys.Space).Gamepad(Buttons.B));
			controls.Add(new Control(Use).Mouse(MouseButtons.Left).Gamepad(Buttons.RightTrigger, Buttons.DPadDown));

			controls.Add(new Control(Bomb, Keys.Q).Gamepad(Buttons.Y));
			controls.Add(new Control(Interact, Keys.E).Gamepad(Buttons.X));
			controls.Add(new Control(Swap, Keys.LeftShift).Gamepad(Buttons.A));
			controls.Add(new Control(Roll).Mouse(MouseButtons.Right).Gamepad(Buttons.LeftTrigger));
			controls.Add(new Control(Duck, Keys.R).Gamepad(Buttons.B));
			controls.Add(new Control(Map, Keys.M).Gamepad(Buttons.LeftStick));

			controls.Add(new Control(Pause, Keys.Escape).Gamepad(Buttons.Back));
			
			controls.Add(new Control(Fullscreen, Keys.F11));
			controls.Add(new Control(Fps, Keys.F2));

			controls.Add(new Control(Cancel, Keys.Escape).Gamepad(Buttons.Back));
			controls.Add(new Control(GameStart, Keys.Space, Keys.Enter, Keys.X).Gamepad(Buttons.X, Buttons.Start));
			
			controls.Add(new Control(UiUp, Keys.W, Keys.Up).Gamepad(Buttons.LeftThumbstickUp, Buttons.RightThumbstickUp, Buttons.DPadUp));
			controls.Add(new Control(UiDown, Keys.S, Keys.Down).Gamepad(Buttons.LeftThumbstickDown, Buttons.RightThumbstickDown, Buttons.DPadDown));
			controls.Add(new Control(UiLeft, Keys.A, Keys.Left).Gamepad(Buttons.LeftThumbstickLeft, Buttons.RightThumbstickLeft, Buttons.DPadLeft));
			controls.Add(new Control(UiRight, Keys.D, Keys.Right).Gamepad(Buttons.LeftThumbstickRight, Buttons.RightThumbstickRight, Buttons.DPadRight));
			controls.Add(new Control(UiAccept).Mouse(MouseButtons.Left, MouseButtons.Right));
			controls.Add(new Control(UiSelect, Keys.Enter).Gamepad(Buttons.A,  Buttons.X, Buttons.Y));
			controls.Add(new Control(UiBack, Keys.Escape).Gamepad(Buttons.Back, Buttons.B));
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
							} else {
								Log.Error($"Unknown key {k}");
							}
						}

						control.Keys = l.ToArray();
					}
					
					if (pair.Value["mouse"].IsJsonArray) {
						var l = new List<MouseButtons>();
						
						foreach (var k in pair.Value["mouse"].AsJsonArray) {
							if (Enum.TryParse<MouseButtons>(k.String(""), out var key)) {
								l.Add(key);
							} else {
								Log.Error($"Unknown mouse button {k}");
							}
						}

						control.MouseButtons = l.ToArray();
					}
					
					if (pair.Value["gamepad"].IsJsonArray) {
						var l = new List<Buttons>();
						
						foreach (var k in pair.Value["gamepad"].AsJsonArray) {
							if (Enum.TryParse<Buttons>(k.String(""), out var key)) {
								l.Add(key);
							} else {
								Log.Error($"Unknown gamepad button {k}");
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

		public static string Find(string id, bool gamepad) {
			var k = "None";
			
			foreach (var c in (custom.Count == 0 ? controls : custom)) {
				if (c.Id == id) {
					if (gamepad) {
						k = c.Buttons[0].ToString();
					} else {
						if (c.Keys != null && c.Keys.Length > 0) {
							k = c.Keys[0].ToString();
						} else if (c.MouseButtons != null && c.MouseButtons.Length > 0) {
							k = c.MouseButtons[0].ToString();
						}
					}
				}
			}
			
			if (k == "Left") {
				k = "LMB";
			} else if (k == "Right") {
				k = "RMB";
			} else if (k == "Middle") {
				k = "MMB";
			} else if (k.Length == 2 && k[0] == 'D') {
				k = k[1].ToString();
			} else if (k.StartsWith("Left")) {
				k = $"Left {k.Substring(4, k.Length - 4)}";
			} else if (k.StartsWith("Right")) {
				k = $"Right {k.Substring(5, k.Length - 5)}";
			} else if (k.EndsWith("Left")) {
				k = $"{k.Substring(0, k.Length - 4)} Left";
			} else if (k.EndsWith("Right")) {
				k = $"{k.Substring(0, k.Length - 5)} Right";
			} else if (k.EndsWith("Down")) {
				k = $"{k.Substring(0, k.Length - 4)} Down";
			} else if (k.EndsWith("Up")) {
				k = $"{k.Substring(0, k.Length - 2)} Up";
			}

			return k;
		}

		public static void Replace(string id, Keys key) {
			foreach (var c in (custom.Count == 0 ? controls : custom)) {
				if (c.Id == id) {
					c.Keys = new[] {key};
					c.MouseButtons = null;
					break;
				}
			}
		}

		public static void Replace(string id, Buttons button) {
			foreach (var c in (custom.Count == 0 ? controls : custom)) {
				if (c.Id == id) {
					c.Buttons = new[] {button};
					break;
				}
			}
		}

		public static void Replace(string id, MouseButtons button) {
			foreach (var c in (custom.Count == 0 ? controls : custom)) {
				if (c.Id == id) {
					c.MouseButtons = new[] {button};
					c.Keys = null;
					break;
				}
			}
		}
	}
}