using System;
using BurningKnight.assets.input;
using BurningKnight.assets.lighting;
using BurningKnight.assets.particle;
using BurningKnight.assets.particle.custom;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.entity.projectile;
using BurningKnight.level.tile;
using BurningKnight.save;
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

		public static float TimeIdle;
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
				if (e is PostHealthModifiedEvent ev && ev.Amount < 0) {
					controller.Rumble(0.5f, 0.2f);
				} else if (e is DiedEvent) {
					controller.Rumble(1f, 0.4f);
				}
			}

			return base.HandleEvent(e);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (GetComponent<BuffsComponent>().Has<FrozenBuff>()) {
				return;
			}
			
			var idle = true;
			var controller = GetComponent<GamepadComponent>().Controller;

			if (controller != null && controller.WasAttached && !controller.Attached) {
				controller.WasAttached = false;
				Engine.Instance.State.Paused = true;
				idle = false;
			}

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
							if (Input.WasPressed(Controls.UiUp, controller, true)) {
								c.Choice -= 1;

								if (c.Choice < 0) {
									c.Choice += c.Options.Length;
								}
							}

							if (Input.WasPressed(Controls.UiDown, controller, true)) {
								c.Choice = (c.Choice + 1) % c.Options.Length;
							}
						}
					}

					if (dd.Saying && !dd.JustStarted) {
						if ((!isAnswer && (Input.WasPressed(Controls.Interact, controller, true) || Input.WasPressed(Controls.UiSelect, controller, true))) || (isAnswer && !a.Focused)) {
							if (dd.DoneSaying) {
								dd.Finish();
							} else {
								dd.Str.FinishTyping();
							}
						}
					}
				}
				
				Accelerate(Vector2.Zero, dt);
				return;
			}
			
			var state = Entity.GetComponent<StateComponent>();
			var duck = state.StateInstance is Player.DuckState;
			
			if (duck) {
				if (Input.WasReleased(Controls.Duck, controller)) {
					idle = false;
					state.Become<Player.IdleState>();
				}
			} else if (Input.WasPressed(Controls.Duck, controller)) {
				idle = false;
				state.Become<Player.DuckState>();
				GlobalSave.Put("control_duck", true);
			}
			
			if (state.StateInstance is Player.RollState r) {
				// Movement tech :) Direction changing
				if (Input.WasPressed(Controls.Swap, controller)) {
					idle = false;
					r.ChangeDirection();
				}
				
				// Movement tech :) Roll cancelling
				if (Input.WasPressed(Controls.Roll, controller)) {
					idle = false;
					state.Become<Player.IdleState>();
				}
			} else if (!duck) {
				var acceleration = new Vector2();

				if (Input.IsDown(Controls.Up, controller)) {
					idle = false;
					acceleration.Y -= 1;
				}			
				
				if (Input.IsDown(Controls.Down, controller)) {
					idle = false;
					acceleration.Y += 1;
				}
			
				if (Input.IsDown(Controls.Left, controller)) {
					idle = false;
					acceleration.X -= 1;
				}
			
				if (Input.IsDown(Controls.Right, controller)) {
					idle = false;
					acceleration.X += 1;
				}

				if (Input.Mouse.CheckMiddleButton) {
					idle = false;
					var a = Entity.AngleTo(Input.Mouse.GamePosition);
					acceleration += new Vector2((float) Math.Cos(a), (float) Math.Sin(a));
				}

				if (controller != null) {
					acceleration += controller.GetLeftStick();
				}

				if (Input.WasPressed(Controls.Roll, controller) && !Send(new PlayerRolledEvent {
					Who = (Player) Entity
				})) {
					idle = false;
					GlobalSave.Put("control_roll", true);
					state.Become<Player.RollState>();
				} else {
					if (acceleration.Length() > 0.1f) {
						state.Become<Player.RunState>();
					} else {
						state.Become<Player.IdleState>();
					}

					if (acceleration.Length() > 0.1f) {
						idle = false;
						acceleration.Normalize();
					}

					Accelerate(acceleration, dt);
				}
			}

			if (BK.StandMode && idle && !Engine.Instance.State.Paused && Run.Depth > 0) {
				TimeIdle += dt;

				if (TimeIdle >= 120f) {
					TimeIdle = 0;
					Log.Info("The game was idle for 120 seconds, restarting");
					GlobalSave.ResetControlKnowldge();
					Run.StartNew(0);
				}
			} else {
				TimeIdle = 0;
			}
		}

		public void Accelerate(Vector2 acceleration, float dt) {
			var body = GetComponent<RectBodyComponent>();				
			var i = GetComponent<TileInteractionComponent>();

			var s = Speed;
			var sp = 20;

			if (i.Touching[(int) Tile.Ice]) {
				sp -= 19;
				s *= 0.25f;
			}

			if (i.Touching[(int) Tile.Cobweb]) {
				s *= 0.8f;
			}

			if (i.Touching[(int) Tile.Water] || i.Touching[(int) Tile.Lava]) {
				s *= 0.9f;
			}

			body.Acceleration = acceleration * s;
			body.Velocity -= body.Velocity * dt * sp - body.Acceleration;
		}
	}
}