using System;
using BurningKnight.assets.particle;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.ui.dialog;
using Lens.assets;
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

		public override bool HandleEvent(Event e) {
			var controller = GetComponent<GamepadComponent>().Controller;
			
			if (controller != null) {
				if (e is HealthModifiedEvent ev && ev.Amount < 0) {
					controller.Rumble(0.5f, 0.1f);
				} else if (e is DiedEvent) {
					controller.Rumble(1f, 0.2f);
				}
			}

			return base.HandleEvent(e);
		}

		public override void Update(float dt) {
			base.Update(dt);

			var controller = GetComponent<GamepadComponent>().Controller;
			var state = Entity.GetComponent<StateComponent>();
			var body = GetComponent<RectBodyComponent>();
			
			if (state.StateInstance is Player.RollState) {
				// Movement tech :) Direction changing
				if (Input.WasPressed(Controls.Swap, controller)) {
					(state.StateInstance as Player.RollState).ChangeDirection();
				}
				
				// Movement tech :) Roll cancelling
				if (Input.WasPressed(Controls.Roll, controller)) {
					state.Become<Player.IdleState>();
				}
			} else {
				var acceleration = new Vector2();
				
				if (Input.IsDown(Controls.Up, controller)) {
					acceleration.Y -= 1;
				}			
				
				if (Input.IsDown(Controls.Down, controller)) {
					acceleration.Y += 1;
				}
			
				if (Input.IsDown(Controls.Left, controller)) {
					acceleration.X -= 1;
				}
			
				if (Input.IsDown(Controls.Right, controller)) {
					acceleration.X += 1;
				}

				if (Input.Mouse.CheckMiddleButton) {
					var a = Entity.AngleTo(Input.Mouse.GamePosition);
					acceleration += new Vector2((float) Math.Cos(a), (float) Math.Sin(a));
				}

				if (controller != null) {
					acceleration += controller.GetLeftStick();
				}

				if (Input.WasPressed(Controls.Roll, controller) && !Send(new PlayerRolledEvent {
					Who = (Player) Entity
				})) {
					GetComponent<DialogComponent>().Start("hello");
					
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