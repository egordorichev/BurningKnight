using System;
using BurningKnight.assets.items;
using Lens.entity.component;
using Lens.entity.component.logic;
using Lens.input;
using Lens.util;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.entity.component {
	public class GamepadComponent : Component {
		public static GamepadData Current;
		
		public GamepadData Controller;

		public string GamepadId;

		static GamepadComponent() {
			Camera.OnShake += () => {
				if (Current == null || !Settings.Vibrate || Settings.Gamepad == null) {
					return;
				}

				var am = Camera.Instance.GetComponent<ShakeComponent>().Amount;

				if (am < 5) {
					return;
				}
				
				var a = Math.Max(1, am / 20f);
				Current.Rumble(a, Math.Max(0.1f, a * 0.5f));
			};
		}
		
		public GamepadComponent() {
			UpdateState();
		}

		public override void Destroy() {
			base.Destroy();
			Controller?.StopRumble();
		}

		private void UpdateState() {
			if (Settings.Gamepad != GamepadId && Settings.Gamepad != null) {
				for (int i = 0; i < 4; i++) {
					var c = GamePad.GetCapabilities(i);
					
					if (c.IsConnected && c.Identifier == Settings.Gamepad) {
						Controller = Input.Gamepads[i];
						GamepadId = Settings.Gamepad;
						Current = Controller;
						
						Log.Info($"Connected {GamePad.GetState(i)}");
						Items.Unlock("bk:gamepad");
						
						break;
					}
				}
				
				Settings.Gamepad = null;
			} else if (Controller == null) {
				for (int i = 0; i < 4; i++) {
					if (GamePad.GetCapabilities(i).IsConnected) {
						Controller = Input.Gamepads[i];
						GamepadId = GamePad.GetCapabilities(i).Identifier;
						Current = Controller;
						
						Settings.Gamepad = GamepadId;
						Log.Info($"Connected {GamePad.GetState(i)}");
						Items.Unlock("bk:gamepad");
						
						return;
					}
				}
				
				Settings.Gamepad = null;
			}
		}

		public override void Update(float dt) {
			base.Update(dt);
			UpdateState();
		}
	}
}