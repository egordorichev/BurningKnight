using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob.blood {
	public class Mother : Mob {
		protected void _Init() {
			{
				HpMax = 16;
				Idle = GetAnimation().Get("idle");
				Hurt = GetAnimation().Get("hurt");
				Killed = GetAnimation().Get("dead");
				Animation = Idle;
			}
		}

		public class IdleState : Mob.State<Mother>  {
			private float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(2f, 6f);
			}

			public override Void Update(float Dt) {
				if (Player.Instance.Room == Self.Room) {
					T += Dt;

					if (Self.CanSee(Player.Instance)) {
						MoveRightTo(Player.Instance, 9f, 4f);
					} else {
						MoveTo(Player.Instance, 9f, 4f);
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

		public class TiredState : Mob.State<Mother>  {
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

		public static Animation Animations = Animation.Make("actor-mother", "-normal");
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Hurt;
		private AnimationData Animation;

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void Init() {
			base.Init();
			this.Body = this.CreateSimpleBody(2, 0, 12, 12, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
		}

		public override Void Render() {
			if (this.Target != null) {
				this.Flipped = this.Target.X < this.X;
			} else {
				if (Math.Abs(this.Velocity.X) > 1f) {
					this.Flipped = this.Velocity.X < 0;
				} 
			}


			if (this.Dead) {
				this.Animation = Killed;
			} else if (this.Invt > 0) {
				this.Animation = Hurt;
			} else {
				this.Animation = Idle;
			}


			this.RenderWithOutline(this.Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			Animation.Update(Dt);
			base.Common();
		}

		protected override Void DeathEffects() {
			base.DeathEffects();

			for (int I = 0; I < Random.NewInt(3, 6); I++) {
				Mob Mob = Random.Chance(70) ? new Zombie() : new BigZombie();
				Mob.X = this.X + Random.NewFloat(-8, 16);
				Mob.Y = this.Y + Random.NewFloat(-8, 16);
				Dungeon.Area.Add(Mob.Add());
			}

			this.PlaySfx("death_clown");
			DeathEffect(Killed);
		}

		protected override Void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			this.PlaySfx("damage_clown");
		}

		protected override State GetAi(string State) {
			switch (State) {
				case "idle": 
				case "roam": 
				case "alerted": {
					return new IdleState();
				}

				case "tired": {
					return new TiredState();
				}
			}

			return base.GetAi(State);
		}

		public Mother() {
			_Init();
		}
	}
}
