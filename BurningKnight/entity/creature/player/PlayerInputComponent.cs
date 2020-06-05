using System;
using BurningKnight.assets;
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

namespace BurningKnight.entity.creature.player {
	public class PlayerInputComponent : Component {
		public static bool EnableUpdates;
		
		private const float Speed = 20f;
		private DialogComponent dialog;
		private bool wasSitting;

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

		public override void Update(float dt) {
			base.Update(dt);
			EnableUpdates = !((Player) Entity).SuperHot;

			if (GetComponent<BuffsComponent>().Has<FrozenBuff>()) {
				EnableUpdates = true;
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
				EnableUpdates = true;
				var dd = dialog?.Dialog;

				if (dd != null) {
					var isAnswer = dialog.Current is AnswerDialog;
					var a = isAnswer ? (AnswerDialog) dialog.Current : null;
					
					if (dd.DoneSaying) {
						if (dialog.Current is ChoiceDialog c) {
							if (Input.WasPressed(Controls.UiUp, controller, true)) {
								c.Choice -= 1;

								if (Settings.UiSfx) {
									Audio.PlaySfx("ui_moving");
								}

								if (c.Choice < 0) {
									c.Choice += c.Options.Length;
								}
							}

							if (Input.WasPressed(Controls.UiDown, controller, true)) {
								c.Choice = (c.Choice + 1) % c.Options.Length;
								
								if (Settings.UiSfx) {
									Audio.PlaySfx("ui_moving");
								}
							}
						}
					}

					if (dd.Saying && !dd.JustStarted) {
						if ((!isAnswer && (Input.WasPressed(Controls.Interact, controller, true) || Input.WasPressed(Controls.UiSelect, controller, true))) || (isAnswer && !a.Focused)) {
							if (dd.DoneSaying) {
								dd.Finish();
								Audio.PlaySfx("ui_moving");
							} else {
								dd.Str.FinishTyping();
								Audio.PlaySfx("ui_moving");
							}
						}
					}
				}
				
				Accelerate(Vector2.Zero, dt);

				/*if (Input.WasPressed(Controls.Pause)) {
					dd?.Close();
					dialog?.OnEnd();
					Audio.PlaySfx("ui_moving");
					InGameState.SkipPause = true;
				} else {*/
					return;
				//}
			}

			if (Run.Depth > 0 && Run.Type != RunType.Daily && Input.Keyboard.WasPressed(Keys.P)) {
				Run.StartNew(1, Run.Type);
				Audio.PlaySfx("ui_moving");
				return;
			}
			
			var state = Entity.GetComponent<StateComponent>();
			var duck = state.StateInstance is Player.DuckState;
			
			if (duck) {
				if (Input.WasReleased(Controls.Duck, controller)) {
					idle = false;

					if (wasSitting) {
						state.Become<Player.SittingState>();
					} else {
						state.Become<Player.IdleState>();
					}
				}
			} else if (Input.WasPressed(Controls.Duck, controller)) {
				idle = false;
				wasSitting = state.StateInstance is Player.SittingState;
				state.Become<Player.DuckState>();
				GlobalSave.Put("control_duck", true);
			}

			if (state.StateInstance is Player.PostRollState) {
				EnableUpdates = true;
			} if (state.StateInstance is Player.RollState r) {
				EnableUpdates = true;
				
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

				if (controller != null && controller.Attached) {
					acceleration += controller.GetLeftStick();
				}

				if (Input.WasPressed(Controls.Roll, controller) && !Send(new PlayerRolledEvent {
					Who = (Player) Entity
				})) {
					EnableUpdates = true;
					idle = false;
					GlobalSave.Put("control_roll", true);
					state.Become<Player.RollState>();
				} else {
					if (acceleration.Length() > 0.1f) {
						EnableUpdates = true;
						state.Become<Player.RunState>();
					} else if (!(state.StateInstance is Player.SittingState || state.StateInstance is Player.SleepingState)) {
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
			if (GetComponent<RectBodyComponent>().Confused) {
				acceleration *= -1;
			}
			
			var body = GetComponent<RectBodyComponent>();	
			var i = GetComponent<TileInteractionComponent>();
			var b = GetComponent<BuffsComponent>();
					
			var s = Speed;
			var sp = 20;

			if (((Player) Entity).Sliding || !b.IceImmunity && i.Touching[(int) Tile.Ice]) {
				sp -= 19;
				s *= 0.25f;
			}

			if (i.Touching[(int) Tile.Cobweb]) {
				s *= 0.6f;
			}

			if (i.Touching[(int) Tile.Water] || i.Touching[(int) Tile.Lava]) {
				s *= 0.9f;
			}

			var ac = acceleration * s;
			var st = (GetComponent<StatsComponent>().Speed);
			
			body.Acceleration = ac * st;
			body.Velocity -= body.Velocity * dt * sp - body.Acceleration;

			if (st > 1) {
				body.Position += ac * (0.5f * dt * (st - 1));
			}
		}

		public override void Destroy() {
			base.Destroy();

			if (Entity.TryGetComponent<RectBodyComponent>(out var body) && body.Body != null) {
				body.Body.LinearVelocity = Vector2.Zero;
				body.Acceleration = Vector2.Zero;
			}

			Entity.GetComponent<StateComponent>().Become<Player.IdleState>();
		}
	}
}