using BurningKnight.entity.component;
using Lens.entity.component;
using Lens.entity.component.logic;
using Lens.input;

namespace BurningKnight.entity.creature.player {
	public class PlayerInputComponent : Component {
		private const float Speed = 25f;

		public override void Update(float dt) {
			base.Update(dt);

			var state = Entity.GetComponent<StateComponent>();
			var body = GetComponent<RectBodyComponent>();
			
			body.Acceleration.X = 0;
			body.Acceleration.Y = 0;

			if (!(state.StateInstance is Player.RollState)) {
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

				if (Input.WasPressed(Controls.Roll)) {
					state.Become<Player.RollState>();
				} else {
					if (body.Acceleration.Length() > 0.1f) {
						state.Become<Player.RunState>();
					} else {
						state.Become<Player.IdleState>();
					}
				}

				body.Velocity -= body.Velocity * dt * 10;
			}
		}
	}
}