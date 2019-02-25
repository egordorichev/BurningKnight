using BurningKnight.entity.creature.player;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.blood {
	public class Zombie : Mob {
		public static Animation Animations = Animation.Make("actor-small-zombie", "-normal");
		private AnimationData Animation;
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Run;

		public Zombie() {
			_Init();
		}

		protected void _Init() {
			{
				HpMax = 16;
				Idle = GetAnimation().Get("idle");
				Run = GetAnimation().Get("run");
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
			W = 8;
			H = 8;
			Body = CreateSimpleBody(0, 0, 8, 8, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public override void Render() {
			if (Math.Abs(Velocity.X) > 1f) Flipped = Velocity.X < 0;

			if (Dead)
				Animation = Killed;
			else if (Invt > 0)
				Animation = Hurt;
			else if (Acceleration.Len2() > 1)
				Animation = Run;
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

		public class IdleState : State<Zombie> {
			private float Delay;

			public override void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(2f, 6f);
			}

			public override void Update(float Dt) {
				if (Player.Instance.Room == Self.Room) {
					T += Dt;

					if (Self.CanSee(Player.Instance))
						MoveRightTo(Player.Instance, 25f, 4f);
					else
						MoveTo(Player.Instance, 25f, 4f);


					if (Self.Room != null && Player.Instance.Room == Self.Room)
						foreach (Mob Mob in All)
							if (Mob != Self && Mob.Room == Self.Room) {
								var X = Mob.X + Mob.W / 2 + Mob.Velocity.X * Dt * 10;
								var Y = Mob.Y + Mob.H / 2 + Mob.Velocity.Y * Dt * 10;
								var D = Self.GetDistanceTo(X, Y);

								if (D < 16) {
									var A = D <= 1 ? Random.NewFloat(Math.PI * 2) : Self.GetAngleTo(X, Y);
									var F = 500 * Dt;
									Self.Velocity.X -= Math.Cos(A) * F;
									Self.Velocity.Y -= Math.Sin(A) * F;
								}
							}

					if (T >= Delay) Self.Become("tired");
				}
			}
		}

		public class TiredState : State<Zombie> {
			private float Delay;

			public override void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(2f, 5f);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (T >= Delay) Self.Become("idle");
			}
		}
	}
}