using System;
using Lens.util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Lens.input {
	public class GamepadData {
		public static bool WasChanged;
		
		public PlayerIndex PlayerIndex;
		public GamePadState PreviousState;
		public GamePadState CurrentState;
		public bool WasAttached;
		public bool Attached;

		public static string[] Identifiers;
		
		private float rumbleStrength;
		private float rumbleTime;

		public GamepadData(PlayerIndex playerIndex) {
			PlayerIndex = playerIndex;
		}

		public bool Idle;

		public bool AnythingIsDown(GamePadState state) {
			var d = state.Buttons;
			
			if (d.A == ButtonState.Pressed || d.B == ButtonState.Pressed || d.X == ButtonState.Pressed || d.Y == ButtonState.Pressed) {
				return true;
			}
			
			var p = state.DPad;

			if (p.Left == ButtonState.Pressed || p.Right == ButtonState.Pressed || p.Up == ButtonState.Pressed || p.Down == ButtonState.Pressed) {
				return true;
			}

			return false;
		}

		public bool AnythingIsDown() {
			return AnythingIsDown(CurrentState);
		}
		
		public void Update(float dt) {
			PreviousState = CurrentState;
			CurrentState = GamePad.GetState(PlayerIndex);

			WasAttached = WasAttached || PreviousState.IsConnected;
			Attached = CurrentState.IsConnected;
			
			if (rumbleTime > 0) {
				rumbleTime -= dt;
				rumbleStrength -= dt;

				if (rumbleTime <= 0 || rumbleStrength < 0) {
					GamePad.SetVibration(PlayerIndex, 0, 0);
				} else {
					GamePad.SetVibration(PlayerIndex, rumbleStrength, rumbleStrength);
				}
			}

			if (Attached != WasAttached) {
				WasChanged = true;
			}

			if (PlayerIndex == PlayerIndex.One) {
				// Log.Info($"X: (pressed) {WasPressed(Buttons.X)}, (down) {IsDown(Buttons.X)}");
			}
		}

		public void Rumble(float strength, float time) {
			if (GamePad.SetVibration(PlayerIndex, strength, strength)) {
				rumbleStrength = strength;
				rumbleTime = time;
			}
		}

		public void StopRumble() {
			GamePad.SetVibration(PlayerIndex, 0, 0);
			rumbleTime = 0;
		}

		#region Gamepad butttons	

		public bool Check(Buttons button, Input.CheckType type) {
			switch (type) {
				case Input.CheckType.PRESSED: return WasPressed(button);
				case Input.CheckType.RELEASED: return WasReleased(button);
				case Input.CheckType.DOWN: return IsDown(button);
			}

			return false;
		}
		
		public bool IsDown(Buttons button) {
			if (button == Buttons.LeftTrigger) {
				return LeftTriggerCheck();
			} else if (button == Buttons.RightTrigger) {
				return RightTriggerCheck();
			}
			
			return CurrentState.IsButtonDown(button);
		}

		public bool WasPressed(Buttons button) {
			if (button == Buttons.LeftTrigger) {
				return LeftTriggerWasPressed();
			} else if (button == Buttons.RightTrigger) {
				return RightTriggerWasPressed();
			}
			
			return CurrentState.IsButtonDown(button) && PreviousState.IsButtonUp(button);
		}

		public bool WasReleased(Buttons button) {
			if (button == Buttons.LeftTrigger) {
				return LeftTriggerWasReleased();
			} else if (button == Buttons.RightTrigger) {
				return RightTriggerWasReleased();
			}
			
			return CurrentState.IsButtonUp(button) && PreviousState.IsButtonDown(button);
		}

		#endregion

		#region Gamepad sticks

		public Vector2 GetLeftStick() {
			var ret = CurrentState.ThumbSticks.Left;
			ret.Y = -ret.Y;

			return ret;
		}

		public Vector2 GetLeftStick(float deadzone) {
			var ret = CurrentState.ThumbSticks.Left;

			if (ret.LengthSquared() < deadzone * deadzone) {
				ret = Vector2.Zero;
			}
			else {
				ret.Y = -ret.Y;
			}

			return ret;
		}

		public Vector2 GetRightStick() {
			var ret = CurrentState.ThumbSticks.Right;
			ret.Y = -ret.Y;

			return ret;
		}

		public Vector2 GetRightStick(float deadzone) {
			var ret = CurrentState.ThumbSticks.Right;

			if (ret.LengthSquared() < deadzone * deadzone) {
				ret = Vector2.Zero;
			} else {
				ret.Y = -ret.Y;
			}

			return ret;
		}

		#endregion

		#region Left stick directions

		public bool LeftStickLeftCheck(float deadzone) {
			return CurrentState.ThumbSticks.Left.X <= -deadzone;
		}

		public bool LeftStickLeftWasPressed(float deadzone) {
			return CurrentState.ThumbSticks.Left.X <= -deadzone && PreviousState.ThumbSticks.Left.X > -deadzone;
		}

		public bool LeftStickLeftWasReleased(float deadzone) {
			return CurrentState.ThumbSticks.Left.X > -deadzone && PreviousState.ThumbSticks.Left.X <= -deadzone;
		}

		public bool LeftStickRightCheck(float deadzone) {
			return CurrentState.ThumbSticks.Left.X >= deadzone;
		}

		public bool LeftStickRightWasPressed(float deadzone) {
			return CurrentState.ThumbSticks.Left.X >= deadzone && PreviousState.ThumbSticks.Left.X < deadzone;
		}

		public bool LeftStickRightWasReleased(float deadzone) {
			return CurrentState.ThumbSticks.Left.X < deadzone && PreviousState.ThumbSticks.Left.X >= deadzone;
		}

		public bool LeftStickDownCheck(float deadzone) {
			return CurrentState.ThumbSticks.Left.Y <= -deadzone;
		}

		public bool LeftStickDownWasPressed(float deadzone) {
			return CurrentState.ThumbSticks.Left.Y <= -deadzone && PreviousState.ThumbSticks.Left.Y > -deadzone;
		}

		public bool LeftStickDownWasReleased(float deadzone) {
			return CurrentState.ThumbSticks.Left.Y > -deadzone && PreviousState.ThumbSticks.Left.Y <= -deadzone;
		}

		public bool LeftStickUpCheck(float deadzone) {
			return CurrentState.ThumbSticks.Left.Y >= deadzone;
		}

		public bool LeftStickUpWasPressed(float deadzone) {
			return CurrentState.ThumbSticks.Left.Y >= deadzone && PreviousState.ThumbSticks.Left.Y < deadzone;
		}

		public bool LeftStickUpWasReleased(float deadzone) {
			return CurrentState.ThumbSticks.Left.Y < deadzone && PreviousState.ThumbSticks.Left.Y >= deadzone;
		}

		public float LeftStickHorizontal(float deadzone) {
			float h = CurrentState.ThumbSticks.Left.X;

			if (Math.Abs(h) < deadzone) {
				return 0;
			}

			return h;
		}

		public float LeftStickVertical(float deadzone) {
			float v = CurrentState.ThumbSticks.Left.Y;

			if (Math.Abs(v) < deadzone) {
				return 0;
			}

			return -v;
		}

		#endregion

		#region Right Stick Directions

		public bool RightStickLeftCheck(float deadzone) {
			return CurrentState.ThumbSticks.Right.X <= -deadzone;
		}

		public bool RightStickLeftWasPressed(float deadzone) {
			return CurrentState.ThumbSticks.Right.X <= -deadzone && PreviousState.ThumbSticks.Right.X > -deadzone;
		}

		public bool RightStickLeftWasReleased(float deadzone) {
			return CurrentState.ThumbSticks.Right.X > -deadzone && PreviousState.ThumbSticks.Right.X <= -deadzone;
		}

		public bool RightStickRightCheck(float deadzone) {
			return CurrentState.ThumbSticks.Right.X >= deadzone;
		}

		public bool RightStickRightWasPressed(float deadzone) {
			return CurrentState.ThumbSticks.Right.X >= deadzone && PreviousState.ThumbSticks.Right.X < deadzone;
		}

		public bool RightStickRightWasReleased(float deadzone) {
			return CurrentState.ThumbSticks.Right.X < deadzone && PreviousState.ThumbSticks.Right.X >= deadzone;
		}

		public bool RightStickUpCheck(float deadzone) {
			return CurrentState.ThumbSticks.Right.Y <= -deadzone;
		}

		public bool RightStickUpWasPressed(float deadzone) {
			return CurrentState.ThumbSticks.Right.Y <= -deadzone && PreviousState.ThumbSticks.Right.Y > -deadzone;
		}

		public bool RightStickUpWasReleased(float deadzone) {
			return CurrentState.ThumbSticks.Right.Y > -deadzone && PreviousState.ThumbSticks.Right.Y <= -deadzone;
		}

		public bool RightStickDownCheck(float deadzone) {
			return CurrentState.ThumbSticks.Right.Y >= deadzone;
		}

		public bool RightStickDownWasPressed(float deadzone) {
			return CurrentState.ThumbSticks.Right.Y >= deadzone && PreviousState.ThumbSticks.Right.Y < deadzone;
		}

		public bool RightStickDownWasReleased(float deadzone) {
			return CurrentState.ThumbSticks.Right.Y < deadzone && PreviousState.ThumbSticks.Right.Y >= deadzone;
		}

		public float RightStickHorizontal(float deadzone) {
			float h = CurrentState.ThumbSticks.Right.X;

			if (Math.Abs(h) < deadzone) {
				return 0;
			}

			return h;
		}

		public float RightStickVertical(float deadzone) {
			float v = CurrentState.ThumbSticks.Right.Y;

			if (Math.Abs(v) < deadzone) {
				return 0;
			}

			return -v;
		}

		#endregion

		#region DPad directions

		public int DPadHorizontal => CurrentState.DPad.Right == ButtonState.Pressed
			? 1
			: (CurrentState.DPad.Left == ButtonState.Pressed ? -1 : 0);

		public int DPadVertical => CurrentState.DPad.Down == ButtonState.Pressed
			? 1
			: (CurrentState.DPad.Up == ButtonState.Pressed ? -1 : 0);

		public Vector2 DPad => new Vector2(DPadHorizontal, DPadVertical);

		public bool DPadLeftCheck => CurrentState.DPad.Left == ButtonState.Pressed;

		public bool DPadLeftWasPressed => CurrentState.DPad.Left == ButtonState.Pressed && PreviousState.DPad.Left == ButtonState.Released;

		public bool DPadLeftWasReleased => CurrentState.DPad.Left == ButtonState.Released && PreviousState.DPad.Left == ButtonState.Pressed;

		public bool DPadRightCheck => CurrentState.DPad.Right == ButtonState.Pressed;

		public bool DPadRightWasPressed => CurrentState.DPad.Right == ButtonState.Pressed && PreviousState.DPad.Right == ButtonState.Released;

		public bool DPadRightWasReleased => CurrentState.DPad.Right == ButtonState.Released && PreviousState.DPad.Right == ButtonState.Pressed;

		public bool DPadUpCheck => CurrentState.DPad.Up == ButtonState.Pressed;

		public bool DPadUpWasPressed => CurrentState.DPad.Up == ButtonState.Pressed && PreviousState.DPad.Up == ButtonState.Released;

		public bool DPadUpWasReleased => CurrentState.DPad.Up == ButtonState.Released && PreviousState.DPad.Up == ButtonState.Pressed;

		public bool DPadDownCheck => CurrentState.DPad.Down == ButtonState.Pressed;

		public bool DPadDownWasPressed => CurrentState.DPad.Down == ButtonState.Pressed && PreviousState.DPad.Down == ButtonState.Released;

		public bool DPadDownWasReleased => CurrentState.DPad.Down == ButtonState.Released && PreviousState.DPad.Down == ButtonState.Pressed;

		#endregion

		#region Lef and right triggers


		public const float TriggerTreshold = 0.05f;
		
		public bool LeftTriggerCheck(float threshold = TriggerTreshold) {
			return CurrentState.Triggers.Left >= threshold;
		}

		public bool LeftTriggerWasPressed(float threshold = TriggerTreshold) {
			return CurrentState.Triggers.Left >= threshold && PreviousState.Triggers.Left < threshold;
		}

		public bool LeftTriggerWasReleased(float threshold = TriggerTreshold) {
			return CurrentState.Triggers.Left < threshold && PreviousState.Triggers.Left >= threshold;
		}

		public bool RightTriggerCheck(float threshold = TriggerTreshold) {
			return CurrentState.Triggers.Right >= threshold;
		}

		public bool RightTriggerWasPressed(float threshold = TriggerTreshold) {
			return CurrentState.Triggers.Right >= threshold && PreviousState.Triggers.Right < threshold;
		}

		public bool RightTriggerWasReleased(float threshold = TriggerTreshold) {
			return CurrentState.Triggers.Right < threshold && PreviousState.Triggers.Right >= threshold;
		}

		#endregion
	}
}