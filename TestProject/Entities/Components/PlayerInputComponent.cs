using Lens.Entities.Components;
using Lens.Entities.Components.Graphics;
using Lens.Inputs;

namespace TestProject.Entities.Components {
	public class PlayerInputComponent : Component {
		public override void Update(float dt) {
			base.Update(dt);

			float speed = 120 * dt;
			bool moving = false;
			
			if (Input.IsDown("left")) {
				Entity.Position.X -= speed;
				moving = true;
			}
			
			if (Input.IsDown("right")) {
				Entity.Position.X += speed;
				moving = true;
			}
			
			if (Input.IsDown("up")) {
				Entity.Position.Y -= speed;
				moving = true;
			}
			
			if (Input.IsDown("down")) {
				Entity.Position.Y += speed;
				moving = true;
			}

			Entity.GetComponent<AnimationComponent>().Animation.Tag = moving ? "run" : "idle";
		}
	}
}