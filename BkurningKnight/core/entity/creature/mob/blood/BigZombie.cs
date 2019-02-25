using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob.blood {
	public class BigZombie : Mob {
		protected void _Init() {
			{
				HpMax = 16;
				Idle = GetAnimation().Get("idle");
				Run = GetAnimation().Get("run");
				Hurt = GetAnimation().Get("hurt");
				HurtDead = GetAnimation().Get("hurt_dead");
				Killed = GetAnimation().Get("dead");
				Appear = GetAnimation().Get("respawn");
				Dissappear = GetAnimation().Get("die");
				Animation = Idle;
			}
		}

		public class IdleState : Mob.State<BigZombie>  {
			private float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(4f, 12f);
			}

			public override Void Update(float Dt) {
				if (Player.Instance.Room == Self.Room) {
					T += Dt;

					if (Self.CanSee(Player.Instance)) {
						MoveRightTo(Player.Instance, 10f, 4f);
					} else {
						MoveTo(Player.Instance, 10f, 4f);
					}


					if (Self.Room != null && Player.Instance.Room == Self.Room) {
						foreach (Mob Mob in Mob.All) {
							if (Mob != Self && Mob.Room == Self.Room) {
								float X = Mob.X + Mob.W / 2 + Mob.Velocity.X * Dt * 10;
								float Y = Mob.Y + Mob.H / 2 + Mob.Velocity.Y * Dt * 10;
								float D = Self.GetDistanceTo(X, Y);

								if (D < 16) {
									float A = D <= 1 ? Random.NewFloat((float) (Math.PI * 2)) : Self.GetAngleTo(X, Y);
									float F = 500 * Dt;
									Self.Velocity.X -= Math.Cos(A) * F;
									Self.Velocity.Y -= Math.Sin(A) * F;
								} 
							} 
						}
					} 

					if (T >= Delay) {
						Self.Become("tired");
					} 
				} 
			}
		}

		public class AppearState : Mob.State<BigZombie>  {
			public override Void OnEnter() {
				base.OnEnter();
				Self.Appear.SetFrame(0);
				Self.Appear.SetPaused(false);
				Self.Appear.SetAutoPause(true);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Self.T >= 0.4f) {
					Self.Become("idle");
				} 
			}
		}

		public class DissappearState : Mob.State<BigZombie>  {
			private float Tt;

			public override Void OnEnter() {
				base.OnEnter();
				LastHit = 0;
				Self.Dissappear.SetFrame(0);
				Self.Dissappear.SetPaused(false);
				Self.Dissappear.SetAutoPause(true);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				Tt += Dt;

				if (Tt >= 0.5f) {
					Tt = 0;
					Self.ModifyHp(+1, null);
				} 

				if (T >= 5f && LastHit >= 3f) {
					Self.Become("appear");
				} 
			}
		}

		public class TiredState : Mob.State<BigZombie>  {
			private float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(2f, 5f);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (T >= Delay) {
					Self.Become("idle");
				} 
			}
		}

		public static Animation Animations = Animation.Make("actor-zombie", "-normal");
		private AnimationData Idle;
		private AnimationData Run;
		private AnimationData Killed;
		private AnimationData Hurt;
		private AnimationData HurtDead;
		private AnimationData Appear;
		private AnimationData Dissappear;
		private AnimationData Animation;
		private float LastHit;

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void Init() {
			base.Init();
			this.Body = this.CreateSimpleBody(3, 0, 16 - 6, 12, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
		}

		public override Void Render() {
			if (Math.Abs(this.Velocity.X) > 1f) {
				this.Flipped = this.Velocity.X < 0;
			} 

			if (this.Dead) {
				this.Animation = Killed;
			} else if (this.Invt > 0) {
				this.Animation = this.State.Equals("dissappear") ? HurtDead : Hurt;
			} else if (this.State.Equals("dissappear")) {
				this.Animation = Dissappear;
			} else if (this.State.Equals("appear")) {
				this.Animation = Appear;
			} else if (this.Acceleration.Len2() > 1) {
				this.Animation = Run;
			} else {
				this.Animation = Idle;
			}


			this.RenderWithOutline(this.Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			LastHit += Dt;
			Animation.Update(Dt);
			base.Common();
		}

		protected override Void DeathEffects() {
			base.DeathEffects();
			this.PlaySfx("death_clown");
			DeathEffect(Killed);
		}

		protected override Void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			LastHit = 0;
			this.PlaySfx("damage_clown");
		}

		protected override Mob.State GetAi(string State) {
			switch (State) {
				case "idle": 
				case "roam": 
				case "alerted": {
					return new IdleState();
				}

				case "tired": {
					return new TiredState();
				}

				case "dissappear": {
					return new DissappearState();
				}

				case "appear": {
					return new AppearState();
				}
			}

			return base.GetAi(State);
		}

		public override Void Die() {
			if (State.Equals("dissappear")) {
				base.Die();
			} else {
				Become("dissappear");
				Hp = HpMax / 2;
			}

		}

		public BigZombie() {
			_Init();
		}
	}
}
