using BurningKnight.entity.creature.player;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.blood {
	public class BigZombie : Mob {
		public static Animation Animations = Animation.Make("actor-zombie", "-normal");
		private AnimationData Animation;
		private AnimationData Appear;
		private AnimationData Dissappear;
		private AnimationData Hurt;
		private AnimationData HurtDead;
		private AnimationData Idle;
		private AnimationData Killed;
		private float LastHit;
		private AnimationData Run;

		public BigZombie() {
			_Init();
		}

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

		public Animation GetAnimation() {
			return Animations;
		}

		public override void Init() {
			base.Init();
			Body = CreateSimpleBody(3, 0, 16 - 6, 12, BodyDef.BodyType.DynamicBody, false);
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
				Animation = State.Equals("dissappear") ? HurtDead : Hurt;
			else if (State.Equals("dissappear"))
				Animation = Dissappear;
			else if (State.Equals("appear"))
				Animation = Appear;
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
			LastHit += Dt;
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
			LastHit = 0;
			PlaySfx("damage_clown");
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

		public override void Die() {
			if (State.Equals("dissappear")) {
				base.Die();
			}
			else {
				Become("dissappear");
				Hp = HpMax / 2;
			}
		}

		public class IdleState : State<BigZombie> {
			private float Delay;

			public override void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(4f, 12f);
			}

			public override void Update(float Dt) {
				if (Player.Instance.Room == Self.Room) {
					T += Dt;

					if (Self.CanSee(Player.Instance))
						MoveRightTo(Player.Instance, 10f, 4f);
					else
						MoveTo(Player.Instance, 10f, 4f);


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

		public class AppearState : State<BigZombie> {
			public override void OnEnter() {
				base.OnEnter();
				Self.Appear.SetFrame(0);
				Self.Appear.SetPaused(false);
				Self.Appear.SetAutoPause(true);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (Self.T >= 0.4f) Self.Become("idle");
			}
		}

		public class DissappearState : State<BigZombie> {
			private float Tt;

			public override void OnEnter() {
				base.OnEnter();
				LastHit = 0;
				Self.Dissappear.SetFrame(0);
				Self.Dissappear.SetPaused(false);
				Self.Dissappear.SetAutoPause(true);
			}

			public override void Update(float Dt) {
				base.Update(Dt);
				Tt += Dt;

				if (Tt >= 0.5f) {
					Tt = 0;
					Self.ModifyHp(+1, null);
				}

				if (T >= 5f && LastHit >= 3f) Self.Become("appear");
			}
		}

		public class TiredState : State<BigZombie> {
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