using BurningKnight.entity.creature.player;
using BurningKnight.physics;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.creature.mob.ice {
	public class Snowflake : Mob {
		public static Animation Animations = Animation.Make("actor-snowflake", "-white");
		private AnimationData Animation;
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;

		public Snowflake() {
			_Init();
		}

		protected void _Init() {
			{
				HpMax = 12;
				Idle = GetAnimation().Get("idle");
				Hurt = GetAnimation().Get("hurt");
				Killed = GetAnimation().Get("dead");
				Animation = Idle;
			}
		}

		public Animation GetAnimation() {
			return Animations;
		}

		public override void Init() {
			base.Init();
			Flying = true;
			Body = World.CreateCircleBody(this, 2, 2, 6, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public override void Render() {
			if (Target != null) {
				Flipped = Target.X < this.X;
			}
			else {
				if (Math.Abs(Velocity.X) > 1f) Flipped = Velocity.X < 0;
			}


			if (Dead)
				Animation = Killed;
			else if (Invt > 0)
				Animation = Hurt;
			else
				Animation = Idle;


			this.RenderWithOutline(Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			Animation.Update(Dt);
			Common();
		}

		protected override void DeathEffects() {
			base.DeathEffects();
			PlaySfx("death_clown");
			DeathEffect(Killed);
		}

		protected override void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			PlaySfx("damage_clown");
		}

		protected override State GetAi(string State) {
			return new IdleState();
		}

		public class IdleState : State<Snowflake> {
			private float Init;

			public override void OnEnter() {
				base.OnEnter();
				Init = Random.NewFloat(Math.PI * 2);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (Self.Target != null && Self.Target.Room == Self.Room) {
					if (Self.CanSee(Self.Target)) {
						var A = T * 1.5f + Init;
						var D = 32f;
						FlyTo(new Point(Self.Target.X + 8 + (float) Math.Cos(A) * D, Self.Target.Y + 8 + (float) Math.Sin(A) * D), 20f, 4f);

						if (Self.Room != null && Player.Instance.Room == Self.Room)
							foreach (Mob Mob in All)
								if (Mob != Self && Mob.Room == Self.Room && Mob is Snowflake) {
									var X = Mob.X + Mob.W / 2 + Mob.Velocity.X * Dt * 10;
									var Y = Mob.Y + Mob.H / 2 + Mob.Velocity.Y * Dt * 10;
									D = Self.GetDistanceTo(X, Y);

									if (D < 16) {
										A = D <= 1 ? Random.NewFloat(Math.PI * 2) : Self.GetAngleTo(X, Y);
										var F = 600 * Dt;
										Self.Velocity.X -= Math.Cos(A) * F;
										Self.Velocity.Y -= Math.Sin(A) * F;
									}
								}
					}
					else {
						MoveTo(Self.Target, 20f, 4f);
					}
				}
			}
		}
	}
}