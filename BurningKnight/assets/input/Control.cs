using Lens.input;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.assets.input {
	public class Control {
		public Keys[] Keys;
		public MouseButtons[] MouseButtons;
		public Buttons[] Buttons;
		public string Id;

		public Control(string id, params Keys[] keys) {
			Id = id;
			Keys = keys;
		}

		public Control Gamepad(params Buttons[] buttons) {
			Buttons = buttons;
			return this;
		}

		public Control Mouse(params MouseButtons[] buttons) {
			MouseButtons = buttons;
			return this;
		}
	}
}