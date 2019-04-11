using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using Lens.entity;
using Lens.entity.component;
using Lens.entity.component.logic;
using Lens.input;
using Lens.util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.entity.creature.player {
	public class PlayerInputComponent : Component {
		private const float Speed = 20f;
		private GamepadData data;
	
		public override void Update(float dt) {
			base.Update(dt);

			if (data == null) {
				for (int i = 0; i < 4; i++) {
					if (Input.Gamepads[i].Attached) {
						data = Input.Gamepads[i];
						Log.Info($"Connecteted {GamePad.GetState(i)}");
						break;
					}
				}
			}
			
			var state = Entity.GetComponent<StateComponent>();
			var body = GetComponent<RectBodyComponent>();
			
			if (!(state.StateInstance is Player.RollState)) {
				var acceleration = new Vector2();
				
				if (Input.IsDown(Controls.Up, data)) {
					acceleration.Y -= 1;
				}			
				
				if (Input.IsDown(Controls.Down, data)) {
					acceleration.Y += 1;
				}
			
				if (Input.IsDown(Controls.Left, data)) {
					acceleration.X -= 1;
				}
			
				if (Input.IsDown(Controls.Right, data)) {
					acceleration.X += 1;
				}

				if (data != null) {
					acceleration += data.GetLeftStick();
				}

				if (Input.WasPressed(Controls.Roll, data) && !Send(new PlayerRolledEvent {
					Who = (Player) Entity
				})) {
					state.Become<Player.RollState>();

					Entity.Area.Add(new BloodFx {
						Position = Entity.Center
					});

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