using BurningKnight.entity.component;
using Lens.entity.component;
using Lens.entity.component.logic;
using Lens.input;
using Lens.util;
using Microsoft.Xna.Framework.Input;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.player {
	public class PlayerInputComponent : Component {
		private const float Speed = 25f;

		public override void Update(float dt) {
			base.Update(dt);

			var body = GetComponent<RectBodyComponent>();

			body.Acceleration.X = 0;
			body.Acceleration.Y = 0;

			if (Input.IsDown(Controls.Up)) {
				body.Acceleration.Y -= Speed;
			}
			
			if (Input.IsDown(Controls.Down)) {
				body.Acceleration.Y += Speed;
			}
			
			if (Input.IsDown(Controls.Left)) {
				body.Acceleration.X -= Speed;
			}
			
			if (Input.IsDown(Controls.Right)) {
				body.Acceleration.X += Speed;
			}

			body.Velocity -= body.Velocity * dt * 10;

			if (body.Acceleration.Length() > 0.1f) {
				Entity.GetComponent<StateComponent>().Become<Player.RunState>();
			} else {
				Entity.GetComponent<StateComponent>().Become<Player.IdleState>();
			}
		}
	}
}