using System;
using ImGuiNET;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace Lens.input {
	public class MouseData {		
		public MouseState PreviousState;
		public MouseState CurrentState;
		public static bool HadClick;

		private bool blockedByGui;
		
		public MouseData() {
			PreviousState = new MouseState();
			CurrentState = new MouseState();
		}

		internal void Update() {
			PreviousState = CurrentState;
			CurrentState = Mouse.GetState();

			blockedByGui = Input.EnableImGuiFocus && ImGui.GetIO().WantCaptureMouse;
			HadClick = HadClick || WasPressedLeftButton;
		}

		#region Buttons

		public bool Check(MouseButtons button, Input.CheckType type) {
			if (blockedByGui) {
				return false;
			}
			
			switch (type) {
				case Input.CheckType.PRESSED: {
					switch (button) {
						case MouseButtons.Left: return WasPressedLeftButton;
						case MouseButtons.Right: return WasPressedRightButton;
						case MouseButtons.Middle: return WasPressedMiddleButton;
					}
					
					return false;
				}
				case Input.CheckType.RELEASED: {
					switch (button) {
						case MouseButtons.Left: return WasReleasedLeftButton;
						case MouseButtons.Right: return WasReleasedRightButton;
						case MouseButtons.Middle: return WasReleasedMiddleButton;
					}
					
					return false;
				}
				case Input.CheckType.DOWN: {
					switch (button) {
						case MouseButtons.Left: return CheckLeftButton;
						case MouseButtons.Right: return CheckRightButton;
						case MouseButtons.Middle: return CheckMiddleButton;
					}
					
					return false;
				}
			}

			return false;
		}
		
		public bool CheckLeftButton => CurrentState.LeftButton == ButtonState.Pressed;
		public bool CheckRightButton => CurrentState.RightButton == ButtonState.Pressed;
		public bool CheckMiddleButton => CurrentState.MiddleButton == ButtonState.Pressed;
		public bool WasPressedLeftButton => CurrentState.LeftButton == ButtonState.Pressed &&
		                                    PreviousState.LeftButton == ButtonState.Released;
		public bool WasPressedRightButton => CurrentState.RightButton == ButtonState.Pressed &&
		                                     PreviousState.RightButton == ButtonState.Released;
		public bool WasPressedMiddleButton => CurrentState.MiddleButton == ButtonState.Pressed &&
		                                      PreviousState.MiddleButton == ButtonState.Released;
		public bool WasReleasedLeftButton => CurrentState.LeftButton == ButtonState.Released &&
		                                     PreviousState.LeftButton == ButtonState.Pressed;
		public bool WasReleasedRightButton => CurrentState.RightButton == ButtonState.Released &&
		                                      PreviousState.RightButton == ButtonState.Pressed;
		public bool WasReleasedMiddleButton => CurrentState.MiddleButton == ButtonState.Released &&
		                                       PreviousState.MiddleButton == ButtonState.Pressed;

		#endregion

		#region Wheel

		public int Wheel => CurrentState.ScrollWheelValue;
		public int WheelDelta => CurrentState.ScrollWheelValue - PreviousState.ScrollWheelValue;

		#endregion

		#region Position

		public bool WasMoved => CurrentState.X != PreviousState.X
			|| CurrentState.Y != PreviousState.Y;
		
		public Vector2 PositionDelta => 
			Vector2.Transform(RawPositionDelta, Matrix.Invert(Matrix.CreateScale(Engine.Instance.Upscale)));
		
		public Vector2 RawPositionDelta => 
			new Vector2(CurrentState.X - PreviousState.X, CurrentState.Y - PreviousState.Y);

		public float X {
			get { return Position.X; }
			set { Position = new Vector2(value, Position.Y); }
		}

		public float Y {
			get { return Position.Y; }
			set { Position = new Vector2(Position.X, value); }
		}

		public Vector2 ScreenPosition => new Vector2(CurrentState.X, CurrentState.Y);
		public Vector2 GamePosition => Camera.Instance.ScreenToCamera(ScreenPosition);
		
		public Vector2 Position {
			get { return Vector2.Transform(new Vector2(CurrentState.X, CurrentState.Y), Matrix.Invert(Engine.ScreenMatrix)); }

			set {
				var vector = Vector2.Transform(value, Engine.ScreenMatrix);
				Mouse.SetPosition((int) Math.Round(vector.X), (int) Math.Round(vector.Y));
			}
		}
		
		public Vector2 UiPosition {
			get { return Vector2.Transform(new Vector2(CurrentState.X, CurrentState.Y), Matrix.Invert(Engine.UiMatrix)); }

			set {
				var vector = Vector2.Transform(value, Engine.UiMatrix);
				Mouse.SetPosition((int) Math.Round(vector.X), (int) Math.Round(vector.Y));
			}
		}

		#endregion
	}
}