using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.item.weapon.sword;
using BurningKnight.core.entity.level;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob.hall {
	public class Knight : Mob {
		protected void _Init() {
			{
				HpMax = 7;
				Speed = 5;
				Idle = GetAnimation().Get("idle").Randomize();
				Run = GetAnimation().Get("run").Randomize();
				Hurt = GetAnimation().Get("hurt").Randomize();
				Killed = GetAnimation().Get("death").Randomize();
				Animation = this.Idle;
			}
		}

		public class KnightState : State<Knight>  {

		}

		public class SawState : KnightState {
			public override Void Update(float Dt) {
				base.Update(Dt);

				if (this.T >= 1f) {
					Self.Become("chase");
				} 
			}
		}

		public class RoamState : KnightState {
			public override Void Update(float Dt) {
				base.Update(Dt);
				this.CheckForPlayer();

				if (Self.Target != null && Self.CanSee(Self.Target)) {
					Self.Saw = true;
					Self.NoticeSignT = 2f;
					Self.PlaySfx("enemy_alert");
					Self.Become("alerted");
				} 
			}
		}

		public class ChaseState : KnightState {
			public const float ATTACK_DISTANCE = 16f;
			public const float DASH_DIST = 48f;
			public float Delay;
			private float Att;

			public override Void OnEnter() {
				base.OnEnter();
				this.Delay = Random.NewFloat(4f, 5f);

				if (Self.Sword is Sword) {
					this.Att = ATTACK_DISTANCE;
				} else {
					this.Att = 180f;
				}

			}

			public override Void Update(float Dt) {
				this.CheckForPlayer();

				if (Self.LastSeen == null) {
					Self.Become("idle");

					return;
				} else {
					if (this.MoveTo(Self.LastSeen, 18f, this.Att)) {
						if (Self.Target != null && Self.GetDistanceTo((int) (Self.Target.X + Self.Target.W / 2), (int) (Self.Target.Y + Self.Target.H / 2)) <= this.Att) {
							if (Self.CanSee(Self.Target)) {
								Self.Become("preattack");
							} 
						} else {
							Self.NoticeSignT = 0f;
							Self.HideSignT = 2f;
							Self.Become("idle");
						}

					} 

					Self.LastAcceleration.X = Self.Acceleration.X;
					Self.LastAcceleration.Y = Self.Acceleration.Y;

					if (this.T >= this.Delay && Self.CanSee(Player.Instance, Terrain.HOLE)) {
						Self.Become("predash");

						return;
					} 
				}


				base.Update(Dt);
			}
		}

		public class AttackingState : KnightState {
			public override Void OnEnter() {
				base.OnEnter();
				Self.Sword.Use();
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Self.Target != null) {
					Self.Flipped = Self.Target.X + Self.Target.W / 2 < Self.X + Self.W / 2;
				} 

				if (Self.Sword.GetDelay() == 0) {
					Self.Become("chase");
					this.CheckForPlayer();
				} 
			}
		}

		public class PreAttackState : KnightState {
			public override Void Update(float Dt) {
				base.Update(Dt);

				if (this.T > 0.2f) {
					Self.Become("attack");
				} 
			}
		}

		public class PredashState : KnightState {
			public override Void OnEnter() {
				base.OnEnter();
				Self.PlaySfx("predash");
				Self.Velocity.X = 0;
				Self.Velocity.Y = 0;
				Tween.To(new Tween.Task(0.7f, 0.5f) {
					public override float GetValue() {
						return Self.Sy;
					}

					public override Void SetValue(float Value) {
						Self.Sy = Value;
					}

					public override Void OnEnd() {
						Tween.To(new Tween.Task(1f, 0.2f) {
							public override Void OnStart() {
								base.OnStart();
								Self.PlaySfx("dash");
								Self.Become("dash");
							}

							public override float GetValue() {
								return Self.Sy;
							}

							public override Void SetValue(float Value) {
								Self.Sy = Value;
							}
						}).Delay(0.1f);
					}
				});
				Tween.To(new Tween.Task(1.3f, 0.5f) {
					public override float GetValue() {
						return Self.Sx;
					}

					public override Void SetValue(float Value) {
						Self.Sx = Value;
					}

					public override Void OnEnd() {
						Tween.To(new Tween.Task(1f, 0.2f) {
							public override float GetValue() {
								return Self.Sx;
							}

							public override Void SetValue(float Value) {
								Self.Sx = Value;
							}
						}).Delay(0.1f);
					}
				});
			}
		}

		public class DashState : KnightState {
			private Vector2 Vel;

			public override Void OnEnter() {
				base.OnEnter();

				for (int I = 0; I < 5; I++) {
					PoofFx Fx = new PoofFx();
					Fx.X = Self.X + Self.W / 2;
					Fx.Y = Self.Y + Self.H / 2;
					Dungeon.Area.Add(Fx);
				}

				float Dx = Self.Target.X + 8 - Self.X - Self.W / 2;
				float Dy = Self.Target.Y + 8 - Self.Y - Self.H / 2;
				float D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);
				this.Vel = new Vector2();
				this.Vel.X = Dx / (D + Random.NewFloat(-D / 3, D / 3)) * 350;
				this.Vel.Y = Dy / (D + Random.NewFloat(-D / 3, D / 3)) * 350;
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				this.Vel.X -= this.Vel.X * Math.Min(1, Dt * 3);
				this.Vel.Y -= this.Vel.Y * Math.Min(1, Dt * 3);
				Self.Velocity.X = this.Vel.X;
				Self.Velocity.Y = this.Vel.Y;

				if (this.T >= 1f) {
					Self.Become("wait");
				} 
			}
		}

		public class WaitState : KnightState {
			public override Void Update(float Dt) {
				base.Update(Dt);

				if (this.T >= 2f) {
					Self.Become("chase");
				} 
			}
		}

		public static Animation Animations = Animation.Make("actor-knight", "-blue");
		private AnimationData Idle;
		private AnimationData Run;
		private AnimationData Hurt;
		private AnimationData Killed;
		private AnimationData Animation;
		protected Item Sword;
		private Vector2 LastAcceleration = new Vector2();
		public float MinAttack = 130f;

		public Animation GetAnimation() {
			return Animations;
		}

		protected override Void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			this.PlaySfx("damage_towelknight");
		}

		public override bool RollBlock() {
			if (!this.State.Equals("preattack") && !this.State.Equals("attack")) {
				this.Become("chase");

				return false;
			} 

			return base.RollBlock();
		}

		public override Void Init() {
			base.Init();
			this.Sword = new Sword();
			this.Sword.SetOwner(this);
			this.Body = this.CreateSimpleBody(2, 1, 12, 12, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (this.Freezed) {
				return;
			} 

			if (this.Dead) {
				base.Common();

				return;
			} 

			if (this.Animation != null) {
				this.Animation.Update(Dt * SpeedMod);
			} 

			this.Sword.Update(Dt * SpeedMod);
			this.Sword.UpdateInHands(Dt * SpeedMod);
			base.Common();
		}

		public override Void Render() {
			if (Math.Abs(this.Velocity.X) > 1f) {
				this.Flipped = this.Velocity.X < 0;
			} 

			float V = Math.Abs(this.Acceleration.X) + Math.Abs(this.Acceleration.Y);

			if (this.Dead) {
				this.Animation = Killed;
			} else if (this.Invt > 0) {
				this.Animation = Hurt;
			} else if (V > 1f) {
				this.Animation = Run;
			} else {
				this.Animation = Idle;
			}


			this.RenderWithOutline(this.Animation);
			Graphics.Batch.SetColor(1, 1, 1, this.A);
			this.Sword.Render(this.X, this.Y, this.W, this.H, this.Flipped, false);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		protected override List GetDrops<Item> () {
			List<Item> Items = base.GetDrops();

			if (Random.Chance(5)) {
				Items.Add(new Sword());
			} 

			return Items;
		}

		public override Void Destroy() {
			base.Destroy();
			this.Sword.Destroy();
		}

		protected override State GetAi(string State) {
			switch (State) {
				case "wait": {
					return new WaitState();
				}

				case "idle": 
				case "roam": {
					return new RoamState();
				}

				case "chase": {
					return new ChaseState();
				}

				case "dash": {
					return new DashState();
				}

				case "preattack": {
					return new PreAttackState();
				}

				case "attack": {
					return new AttackingState();
				}

				case "alerted": {
					return new SawState();
				}

				case "predash": {
					return new PredashState();
				}
			}

			return base.GetAi(State);
		}

		public override Void RenderSigns() {
			base.RenderSigns();

			if (this.Sword is Gun) {
				((Gun) this.Sword).RenderReload();
			} 
		}

		public override Void RenderShadow() {
			Graphics.ShadowSized(this.X, this.Y, this.W, this.H, 6);
		}

		public Void CheckDir() {

		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (!(Entity is Mob)) {
				CheckDir();
			} 

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		public override float GetWeaponAngle() {
			return (float) (Math.Atan2(LastAcceleration.Y, LastAcceleration.X) + Math.PI / 2 * (this.Flipped ? 1 : -1));
		}

		protected override Void DeathEffects() {
			base.DeathEffects();
			this.PlaySfx("death_towelknight");
			DeathEffect(Killed);
		}

		public Knight() {
			_Init();
		}
	}
}
