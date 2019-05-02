using Lens.entity.component;
using Lens.input;
using Lens.util;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.entity.component {
	public class GamepadComponent : Component {
		public GamepadData Controller;

		public override void Update(float dt) {
			base.Update(dt);
			
			if (Controller == null) {
				for (int i = 0; i < 4; i++) {
					if (Input.Gamepads[i].Attached) {
						Controller = Input.Gamepads[i];
						Log.Info($"Connected {GamePad.GetState(i)}");
						break;
					}
				}
			}
		}
	}
}