using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature.mob.ice {
	public class Snowflake : Mob {
		protected void _Init() {
			{
				HpMax = 12;
				Idle = GetAnimation().Get("idle");
				Hurt = GetAnimation().Get("hurt");
				Killed = GetAnimation().Get("dead");
				Animation = Idle;
			}
		}

		public class IdleState : Mob.State<Snowflake>  {
			private float Init;

			public override Void OnEnter() {
				base.OnEnter();
				Init = Random.NewFloat((float) (Math.PI * 2));
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Self.Target != null && Self.Target.Room == Self.Room) {
					if (Self.CanSee(Self.Target)) {
						float A = T * 1.5f + Init;
						float D = 32f;
						FlyTo(new Point(Self.Target.X + 8 + (float) Math.Cos(A) * D, Self.Target.Y + 8 + (float) Math.Sin(A) * D), 20f, 4f);

						if (Self.Room != null && Player.Instance.Room == Self.Room) {
							foreach (Mob Mob in Mob.All) {
								if (Mob != Self && Mob.Room == Self.Room && Mob is Snowflake) {
									float X = Mob.X + Mob.W / 2 + Mob.Velocity.X * Dt * 10;
									float Y = Mob.Y + Mob.H / 2 + Mob.Velocity.Y * Dt * 10;
									D = Self.GetDistanceTo(X, Y);

									if (D < 16) {
										A = D <= 1 ? Random.NewFloat((float) (Math.PI * 2)) : Self.GetAngleTo(X, Y);
										float F = 600 * Dt;
										Self.Velocity.X -= Math.Cos(A) * F;
										Self.Velocity.Y -= Math.Sin(A) * F;
									} 
								} 
							}
						} 
					} else {
						MoveTo(Self.Target, 20f, 4f);
					}

				} 
			}
		}

		public static Animation Animations = Animation.Make("actor-snowflake", "-white");
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Hurt;
		private AnimationData Animation;

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void Init() {
			base.Init();
			Flying = true;
			this.Body = World.CreateCircleBody(this, 2, 2, 6, BodyDef.BodyType.DynamicBody, false);
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
			this.PlaySfx("death_clown");
			DeathEffect(Killed);
		}

		protected override Void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			this.PlaySfx("damage_clown");
		}

		protected override State GetAi(string State) {
			return new IdleState();
		}

		public Snowflake() {
			_Init();
		}
	}
}
