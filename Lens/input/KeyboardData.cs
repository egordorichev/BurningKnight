using ImGuiNET;
using Microsoft.Xna.Framework.Input;

namespace Lens.input {
	public class KeyboardData {
		public KeyboardState PreviousState;
		public KeyboardState State;

		private bool guiBlocksKeyboard;

		public KeyboardData() {
			State = Keyboard.GetState();
		}
		
		public void Update() {
			PreviousState = State;
			State = Keyboard.GetState();

			guiBlocksKeyboard = ImGui.GetIO().WantCaptureKeyboard;
		}

		public bool Check(Keys key, Input.CheckType type, bool ignoreGui = false) {
			switch (type) {
				case Input.CheckType.PRESSED: return WasPressed(key, ignoreGui);
				case Input.CheckType.RELEASED: return WasReleased(key, ignoreGui);
				case Input.CheckType.DOWN: return IsDown(key, ignoreGui);
			}

			return false;
		}

		public bool IsDown(Keys key, bool ignoreGui = false) {
			return (ignoreGui || !guiBlocksKeyboard) && State.IsKeyDown(key);
		}

		public bool WasPressed(Keys key, bool ignoreGui = false) {
			return (ignoreGui || !guiBlocksKeyboard) && State.IsKeyDown(key) && !PreviousState.IsKeyDown(key);
		}
		
		public bool WasReleased(Keys key, bool ignoreGui = false) {
			return (ignoreGui || !guiBlocksKeyboard) && !State.IsKeyDown(key) && PreviousState.IsKeyDown(key);
		}
	}
}