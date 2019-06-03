using System;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.entity.component;
using Lens.entity.component.logic;
using Lens.graphics;
using Lens.input;
using Lens.util;
using Lens.util.camera;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.player {
	public class PlayerInputComponent : Component {
		private const float Speed = 20f;
		private DialogComponent dialog;
		
		public bool InDialog;

		public DialogComponent Dialog {
			get => dialog;
			
			set {
				var old = dialog;
				dialog = value;

				if (dialog == null) {
					if (old != null) {
						((InGameState) Engine.Instance.State).ResetFollowing();
						// Tween.To(1f, Camera.Instance.TextureZoom, x => Camera.Instance.TextureZoom = x, 1f);
					}
				} else if (dialog != null) {
					Camera.Instance.Targets.Clear();

					if (old == null) {				
						// Tween.To(2f, Camera.Instance.TextureZoom, x => Camera.Instance.TextureZoom = x, 0.35f);
					}
					
					Camera.Instance.Follow(dialog.Entity, 1f);
				}
			}
		}

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

			if (InDialog) {
				if (Input.WasPressed(Controls.Cancel)) {
					InDialog = false;
					dialog?.Close();
					
					return;
				}

				var dd = dialog?.Dialog;

				if (dd != null) {
					var isAnswer = dialog.Current is AnswerDialog;
					var a = isAnswer ? (AnswerDialog) dialog.Current : null;
					
					if (dd.DoneSaying) {
						if (dialog.Current is ChoiceDialog c) {
							if (Input.WasPressed(Controls.Up, controller, true)) {
								c.Choice -= 1;

								if (c.Choice < 0) {
									c.Choice += c.Options.Length;
								}
							}

							if (Input.WasPressed(Controls.Down, controller, true)) {
								c.Choice = (c.Choice + 1) % c.Options.Length;
							}
						} else if (isAnswer) {
							// fixme: input text
						}
					}

					if (dd.Saying) {
						if ((!isAnswer && Input.WasPressed(Controls.Interact, controller, true)) || (isAnswer && !a.Focused)) {
							if (dd.DoneSaying) {
								dd.Finish();
							} else {
								dd.Str.FinishTyping();
							}
						}
					}
				}
				
				return;
			}
			
			if (Input.Keyboard.WasPressed(Keys.Space, true)) {
				var explosion = new ParticleEntity(Particles.Animated("spawn_fx"));
				explosion.Position = Entity.Center;
				Entity.Area.Add(explosion);

				explosion.AddComponent(new LightComponent(explosion, 64f, new Color(1f, 0.1f, 0.1f, 0.8f)));
				explosion.Depth = 31;
				explosion.Particle.Velocity = Vector2.Zero;
				explosion.Particle.AngleVelocity = 0;
			}
			
			var state = Entity.GetComponent<StateComponent>();
			var body = GetComponent<RectBodyComponent>();
			
			if (state.StateInstance is Player.RollState r) {
				// Movement tech :) Direction changing
				if (Input.WasPressed(Controls.Swap, controller)) {
					r.ChangeDirection();
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

				var got = state.StateInstance is Player.GotState;

				if (!got && Input.WasPressed(Controls.Roll, controller) && !Send(new PlayerRolledEvent {
					Who = (Player) Entity
				})) {
					state.Become<Player.RollState>();
				} else {
					if (!got) {
						if (acceleration.Length() > 0.1f) {
							state.Become<Player.RunState>();
						} else {
							state.Become<Player.IdleState>();
						}
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