using Lens.Entities.Components;
using Lens.Inputs;

namespace TestProject.Entities.Components {
	public class PlayerInputComponent : Component {
		public override void Update(float dt) {
			base.Update(dt);

			float speed = 120 * dt;

			if (Input.IsDown("left")) {
				Entity.Position.X -= speed;
			}
			
			if (Input.IsDown("right")) {
				Entity.Position.X += speed;
			}
			
			if (Input.IsDown("up")) {
				Entity.Position.Y -= speed;
			}
			
			if (Input.IsDown("down")) {
				Entity.Position.Y += speed;
			}
		}
	}
}