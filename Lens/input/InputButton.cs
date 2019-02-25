using System.Collections.Generic;
using Microsoft.Xna.Framework.Input;

namespace Lens.input {
	public struct InputButton {
		public List<Keys> Keys;
		public List<Buttons> Buttons;
		public List<MouseButtons> MouseButtons;
	}
}