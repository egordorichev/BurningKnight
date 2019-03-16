using BurningKnight.entity.component;
using Lens.entity.component;
using Lens.entity.component.logic;
using Lens.input;
using Lens.util.camera;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.player {
	public class PlayerInputComponent : Component {
		private const float Speed = 20f;

		public override void Update(float dt) {
			base.Update(dt);

			var state = Entity.GetComponent<StateComponent>();
			var body = GetComponent<RectBodyComponent>();
			
			if (!(state.StateInstance is Player.RollState)) {
				var acceleration = new Vector2();
				
				if (Input.IsDown(Controls.Up)) {
					acceleration.Y -= 1;
				}
			
				if (Input.IsDown(Controls.Down)) {
					acceleration.Y += 1;
				}
			
				if (Input.IsDown(Controls.Left)) {
					acceleration.X -= 1;
				}
			
				if (Input.IsDown(Controls.Right)) {
					// acceleration.X += 1;
					Camera.Instance.GetComponent<ShakeComponent>().Amount += 1f;
				}

				if (Input.WasPressed(Controls.Roll)) {
					state.Become<Player.RollState>();
				} else {
					if (acceleration.Length() > 0.1f) {
						state.Become<Player.RunState>();
					} else {
						state.Become<Player.IdleState>();
					}
					
					if (acceleration.Length() > 0.1f) {
						acceleration.Normalize();
					}
				
					body.Acceleration = acceleration * Speed;
					body.Velocity -= body.Velocity * dt * 20 - body.Acceleration;
				}
			}
		}
	}
}