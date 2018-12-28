using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Lens.Inputs {
	public static class Input {
		public static KeyboardData Keyboard;
		public static GamepadData[] Gamepads;
		public static PlayerIndex GamepadIndex = PlayerIndex.One;
		
		private static Dictionary<string, InputButton> Buttons = new Dictionary<string, InputButton>();
		
		public static void Init() {
			Keyboard = new KeyboardData();
			Gamepads = new GamepadData[4];

			for (int i = 0; i < Gamepads.Length; i++) {
				Gamepads[i] = new GamepadData((PlayerIndex) i);
			}
		}

		public static void Destroy() {
			foreach (var gamepad in Gamepads) {
				gamepad.StopRumble();
			}
		}

		public static void Update() {
			Keyboard.Update();

			foreach (var gamepad in Gamepads) {
				gamepad.Update();
			}
		}
		
		public static void Bind(string id, params Keys[] values) {
			var button = Buttons.ContainsKey(id) ? Buttons[id] : new InputButton();

			if (button.Keys == null) {
				button.Keys = new List<Keys>();
			}
			
			foreach (var key in values) {
				button.Keys.Add(key);
			}
			
			Buttons[id] = button;
		}
		
		public static void Bind(string id, params Buttons[] values) {
			var button = Buttons.ContainsKey(id) ? Buttons[id] : new InputButton();

			if (button.Buttons == null) {
				button.Buttons = new List<Buttons>();
			}
			
			foreach (var b in values) {
				button.Buttons.Add(b);
			}
			
			Buttons[id] = button;
		}

		public static bool WasPressed(string id) {
			InputButton button;

			if (!Buttons.TryGetValue(id, out button)) {
				return false;
			}

			if (button.Keys != null) {
				foreach (var key in button.Keys) {
					if (Keyboard.WasPressed(key)) {
						return true;
					}
				}
			}

			if (button.Buttons != null) {
				foreach (var b in button.Buttons) {
					if (Gamepads[(int) GamepadIndex].WasPressed(b)) {
						return true;
					}
				}
			}

			return false;
		}
	}
}