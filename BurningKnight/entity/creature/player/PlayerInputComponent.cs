using System;
using BurningKnight.assets.particle;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using Lens.entity;
using Lens.entity.component;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.input;
using Lens.util;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Random = Lens.util.math.Random;

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
						Log.Info($"Connected {GamePad.GetState(i)}");
						break;
					}
				}
			}
			
			var state = Entity.GetComponent<StateComponent>();
			var body = GetComponent<RectBodyComponent>();
			
			if (state.StateInstance is Player.RollState) {
				// Movement tech :) Direction changing
				if (Input.WasPressed(Controls.Swap, data)) {
					(state.StateInstance as Player.RollState).ChangeDirection();
				}
				
				// Movement tech :) Roll cancelling
				if (Input.WasPressed(Controls.Roll, data)) {
					state.Become<Player.IdleState>();
				}
			} else {
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

				if (Input.Mouse.CheckMiddleButton) {
					var a = Entity.AngleTo(Input.Mouse.GamePosition);
					acceleration += new Vector2((float) Math.Cos(a), (float) Math.Sin(a));
				}

				if (data != null) {
					acceleration += data.GetLeftStick();
				}

				if (Input.WasPressed(Controls.Roll, data) && !Send(new PlayerRolledEvent {
					Who = (Player) Entity
				})) {
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