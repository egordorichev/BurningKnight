using Microsoft.Xna.Framework.Input;

namespace Lens.Inputs {
	public class KeyboardData {
		public KeyboardState PreviousState;
		public KeyboardState State;

		public KeyboardData() {
			State = Keyboard.GetState();
		}
		
		public void Update() {
			PreviousState = State;
			State = Keyboard.GetState();
		}

		public bool IsDown(Keys key) {
			return State.IsKeyDown(key);
		}

		public bool WasPressed(Keys key) {
			return State.IsKeyDown(key) && !PreviousState.IsKeyDown(key);
		}
		
		public bool WasReleased(Keys key) {
			return !State.IsKeyDown(key) && PreviousState.IsKeyDown(key);
		}
	}
}