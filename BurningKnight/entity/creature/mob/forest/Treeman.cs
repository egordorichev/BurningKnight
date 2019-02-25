using BurningKnight.entity.creature.player;
using BurningKnight.entity.level.entities;
using BurningKnight.entity.trap;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.forest {
	public class Treeman : Mob {
		public static Animation Animations = Animation.Make("actor-treeman", "-green");
		private AnimationData Animation;
		private AnimationData Down;
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;
		private bool NoLeft;
		private AnimationData Run;
		private AnimationData Up;

		public Treeman() {
			_Init();
		}

		protected void _Init() {
			{
				HpMax = 10;
				Idle = GetAnimation().Get("blink");
				Hurt = GetAnimation().Get("hurt");
				Killed = GetAnimation().Get("dead");
				Up = GetAnimation().Get("up");
				Down = GetAnimation().Get("down");
				Run = GetAnimation().Get("run");
				Animation = Idle;
				W = 19;
			}
		}

		public Animation GetAnimation() {
			return Animations;
		}

		public override void Init() {
			base.Init();
			Flying = true;
			Body = CreateSimpleBody(2, 3, 12, 9, BodyDef.BodyType.DynamicBody, false);
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
			else if (State.Equals("run"))
				Animation = Run;
			else if (State.Equals("up"))
				Animation = Up;
			else if (State.Equals("down"))
				Animation = Down;
			else
				Animation = Idle;


			this.RenderWithOutline(Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is SolidProp || Entity is RollingSpike) return false;

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (!Freezed && !(State.Equals("up") || State.Equals("down") || State.Equals("idle"))) Animation.Update(Dt);

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
				case "up": {
					return new UpState();
				}

				case "down": {
					return new DownState();
				}

				case "run": {
					return new RunState();
				}

				case "idle":
				case "roam":
				case "alerted": {
					return new IdleState();
				}
			}

			return base.GetAi(State);
		}

		public class TreeState : State<Treeman> {
		}

		public class IdleState : TreeState {
			private float Delay;
			private float Dl;

			public override void OnEnter() {
				base.OnEnter();
				T = Random.NewFloat(10);
				Delay = Random.NewFloat(20, 40);
				Dl = Random.NewFloat(7f, 20f);
			}

			public override void Update(float Dt) {
				base.Update(Dt);
				Delay -= Dt;

				if (Self.Target != null && Self.GetDistanceTo(Self.Target.X + 8, Self.Target.Y + 8) < 64) {
					Self.Become("up");

					return;
				}

				var Found = false;

				foreach (Mob Mob in All)
					if (Mob.Room == Self.Room && !(Mob is Treeman)) {
						Found = true;

						break;
					}

				if (!Found) {
					Self.Become("up");
					Self.NoLeft = true;

					return;
				}

				if (T < Dl) {
					Self.Idle.SetFrame(0);

					if (Delay <= 0) Self.Become("up");
				}
				else if (T < Dl + 1.2f) {
					Self.Idle.SetFrame((int) ((T - Dl) * 5));
				}
				else {
					T = 0;
					Dl = Random.NewFloat(7f, 20f);
				}
			}
		}

		public class RunState : TreeState {
			private float Delay;
			private float S;

			public override void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(7, 16f);
				S = Random.NewFloat(16, 40);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (Self.Room != null && Player.Instance.Room == Self.Room)
					foreach (Mob Mob in All)
						if (Mob != Self && Mob.Room == Self.Room && Mob is Treeman) {
							var X = Mob.X + Mob.W / 2 + Mob.Velocity.X * Dt * 10;
							var Y = Mob.Y + Mob.H / 2 + Mob.Velocity.Y * Dt * 10;
							var D = Self.GetDistanceTo(X, Y);

							if (D < 16) {
								var A = D <= 1 ? Random.NewFloat(Math.PI * 2) : Self.GetAngleTo(X, Y);
								var F = 600 * Dt;
								Self.Velocity.X -= Math.Cos(A) * F;
								Self.Velocity.Y -= Math.Sin(A) * F;
							}
						}

				if (!NoLeft) {
					if (Self.Target == null || !Self.OnScreen || Self.GetDistanceTo(Self.Target.X + 8, Self.Target.Y + 8) > 140f) {
						Self.Become("down");

						return;
					}

					if (T >= Delay) {
						Self.Become("down");

						return;
					}
				}

				MoveTo(Player.Instance, S, 4f);
			}
		}

		public class UpState : TreeState {
			public override void Update(float Dt) {
				base.Update(Dt);

				if (T <= 0.5f)
					Self.Up.SetFrame((int) (T * 8f));
				else
					Self.Become("run");
			}
		}

		public class DownState : TreeState {
			public override void Update(float Dt) {
				base.Update(Dt);

				if (T <= 0.5f)
					Self.Down.SetFrame((int) (T * 8f));
				else
					Self.Become("idle");
			}
		}
	}
}