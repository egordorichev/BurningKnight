using System;
using BurningKnight.entity.component;
using Lens.entity.component;
using Lens.entity.component.logic;
using Lens.input;
using Lens.util.camera;
using MonoGame.Extended;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.player {
	public class PlayerInputComponent : Component {
		private const float Speed = 25f;
		private const float RollForce = 400f;

		public override void Update(float dt) {
			base.Update(dt);

			var state = Entity.GetComponent<StateComponent>();
			var body = GetComponent<RectBodyComponent>();
			
			body.Acceleration.X = 0;
			body.Acceleration.Y = 0;

			if (state.StateInstance is Player.RollState) {
				
			} else {
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
					float angle = body.Acceleration.LengthSquared() > 0.1f 
						?	body.Acceleration.ToAngle() 
						: (Camera.Instance.ScreenToCamera(Input.Mouse.ScreenPosition) - Entity.Center).ToAngle();

					body.Acceleration.X += (float) Math.Cos(angle) * RollForce;
					body.Acceleration.Y += (float) Math.Sin(angle) * RollForce;
				} else {
					if (body.Acceleration.Length() > 0.1f) {
						state.Become<Player.RunState>();
					} else {
						state.Become<Player.IdleState>();
					}
				}
			}

			body.Velocity -= body.Velocity * dt * 10;
		}
	}
}