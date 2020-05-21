using Microsoft.Xna.Framework.Input;
using Pico8Emulator.backend;
using Pico8Emulator.unit.input;

namespace MonoGamePico8.backend {
	public class MonoGameInputBackend : InputBackend {
		private static Keys[] keymap = {
				Keys.Left, Keys.Right, Keys.Up, Keys.Down, Keys.Space, Keys.Z,
				// Second variant
				Keys.A, Keys.D, Keys.W, Keys.S, Keys.X, Keys.C
		};

		/*
		 * TODO: implement other player support and gamepad support
		 */
		public override bool IsButtonDown(int i, int p) {
			if (p != 0) return false;
			var s = Keyboard.GetState();
			return s.IsKeyDown(keymap[i]) || s.IsKeyDown(keymap[i + InputUnit.ButtonCount]);
		}
	}
}