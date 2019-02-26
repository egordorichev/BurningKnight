using Lens.entity.component;
using Lens.input;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.entity.creature.player {
	public class PlayerInputComponent : Component {
		public PlayerInputComponent() {
			Input.Bind("up", Keys.W);
			Input.Bind("left", Keys.A);
			Input.Bind("down", Keys.S);
			Input.Bind("right", Keys.D);
			
			Input.Bind("active", Keys.Space);
			Input.Bind("swap", Keys.LeftShift);
			
			Input.Bind("use", MouseButtons.Left);
			Input.Bind("roll", MouseButtons.Right);
		}

		public override void Update(float dt) {
			base.Update(dt);

			// todo
		}
	}
}