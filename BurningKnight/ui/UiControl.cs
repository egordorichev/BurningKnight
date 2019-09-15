using BurningKnight.assets.input;
using BurningKnight.entity.component;
using Lens.assets;
using Lens.input;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.ui {
	public class UiControl : UiButton {
		public static UiControl Focused;
		public string Key;
		public bool Gamepad;
		public GamepadComponent GamepadComponent;

		private float cx;

		public override void Init() {
			base.Init();

			cx = RelativeCenterX;
			SetLabel();
		}

		public override void Destroy() {
			base.Destroy();

			if (Focused == this) {
				Focused = null;
			}
		}

		protected override void OnClick() {
			base.OnClick();

			if (Focused == this) {
				Focused = null;
				SetLabel();
			} else {
				Focused = this;
				Label = $"{Locale.Get(Key)}: {Locale.Get("select")}";
				RelativeCenterX = cx;
			}
		}

		/*
		 * gamepad dpad doesnt work??
		 * centering is off
		 */
		private void SetLabel() {
			var k = Controls.Find(Key, Gamepad);

			if (k.Length == 2 && k[0] == 'D') {
				k = k[1].ToString();
			} else if (k.StartsWith("Left")) {
				k = $"Left {k.Substring(4, k.Length - 4)}";
			}
			
			Label = $"{Locale.Get(Key)}: {k}";
			RelativeCenterX = cx;
		}

		private static Keys[] keysToCheck = {
			Keys.Q, Keys.W, Keys.E, Keys.R, Keys.T, Keys.Y, Keys.U, Keys.I, Keys.O, Keys.P,
			Keys.A, Keys.S, Keys.D, Keys.F, Keys.G, Keys.H, Keys.J, Keys.K, Keys.L,
			Keys.Z, Keys.X, Keys.C, Keys.V, Keys.B, Keys.N, Keys.M, 
			Keys.D0, Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5, Keys.D6, Keys.D7, Keys.D9,
			Keys.Space, Keys.LeftShift, Keys.LeftControl, Keys.LeftWindows, Keys.LeftAlt, Keys.Tab
		};

		private static MouseButtons[] mouseToCheck = {
			MouseButtons.Left, MouseButtons.Middle, MouseButtons.Right
		};

		private static Buttons[] buttonsToCheck = {
			Buttons.A, Buttons.B, Buttons.X, Buttons.Y, Buttons.LeftShoulder, Buttons.RightShoulder,
			Buttons.LeftStick, Buttons.RightStick, Buttons.LeftTrigger, Buttons.RightTrigger,
			Buttons.DPadDown, Buttons.DPadUp, Buttons.DPadLeft, Buttons.DPadRight
		};

		public void Cancel() {
			Focused = null;
			SetLabel();
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Focused == this) {
				if (Gamepad) {
					foreach (var b in buttonsToCheck) {
						if (GamepadComponent.Controller.WasPressed(b)) {
							Controls.Replace(Key, b);
							Controls.Bind();
							Controls.Save();
						
							Focused = null;
							SetLabel();
						
							break;
						}
					}
				} else {
					foreach (var k in keysToCheck) {
						if (Input.Keyboard.WasPressed(k)) {
							
							Controls.Replace(Key, k);
							Controls.Bind();
							Controls.Save();
						
							Focused = null;
							SetLabel();
						
							break;
						}
					}
					
					foreach (var b in mouseToCheck) {
						if (Input.Mouse.Check(b, Input.CheckType.PRESSED)) {
							
							Controls.Replace(Key, b);
							Controls.Bind();
							Controls.Save();
						
							Focused = null;
							SetLabel();
						
							break;
						}
					}
				}
			}
		}
	}
}