using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.level.entities;
using BurningKnight.core.entity.trap;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob.forest {
	public class Treeman : Mob {
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

		public class TreeState : Mob.State<Treeman>  {

		}

		public class IdleState : TreeState {
			private float Delay;
			private float Dl;

			public override Void OnEnter() {
				base.OnEnter();
				T = Random.NewFloat(10);
				Delay = Random.NewFloat(20, 40);
				Dl = Random.NewFloat(7f, 20f);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				Delay -= Dt;

				if (Self.Target != null && Self.GetDistanceTo(Self.Target.X + 8, Self.Target.Y + 8) < 64) {
					Self.Become("up");

					return;
				} 

				bool Found = false;

				foreach (Mob Mob in Mob.All) {
					if (Mob.Room == Self.Room && !(Mob is Treeman)) {
						Found = true;

						break;
					} 
				}

				if (!Found) {
					Self.Become("up");
					Self.NoLeft = true;

					return;
				} 

				if (T < Dl) {
					Self.Idle.SetFrame(0);

					if (Delay <= 0) {
						Self.Become("up");
					} 
				} else if (T < Dl + 1.2f) {
					Self.Idle.SetFrame((int) ((T - Dl) * 5));
				} else {
					T = 0;
					Dl = Random.NewFloat(7f, 20f);
				}

			}
		}

		public class RunState : TreeState {
			private float S;
			private float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(7, 16f);
				S = Random.NewFloat(16, 40);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Self.Room != null && Player.Instance.Room == Self.Room) {
					foreach (Mob Mob in Mob.All) {
						if (Mob != Self && Mob.Room == Self.Room && Mob is Treeman) {
							float X = Mob.X + Mob.W / 2 + Mob.Velocity.X * Dt * 10;
							float Y = Mob.Y + Mob.H / 2 + Mob.Velocity.Y * Dt * 10;
							float D = Self.GetDistanceTo(X, Y);

							if (D < 16) {
								float A = D <= 1 ? Random.NewFloat((float) (Math.PI * 2)) : Self.GetAngleTo(X, Y);
								float F = 600 * Dt;
								Self.Velocity.X -= Math.Cos(A) * F;
								Self.Velocity.Y -= Math.Sin(A) * F;
							} 
						} 
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
			public override Void Update(float Dt) {
				base.Update(Dt);

				if (T <= 0.5f) {
					Self.Up.SetFrame((int) (T * 8f));
				} else {
					Self.Become("run");
				}

			}
		}

		public class DownState : TreeState {
			public override Void Update(float Dt) {
				base.Update(Dt);

				if (T <= 0.5f) {
					Self.Down.SetFrame((int) (T * 8f));
				} else {
					Self.Become("idle");
				}

			}
		}

		public static Animation Animations = Animation.Make("actor-treeman", "-green");
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Hurt;
		private AnimationData Run;
		private AnimationData Up;
		private AnimationData Down;
		private AnimationData Animation;
		private bool NoLeft;

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void Init() {
			base.Init();
			Flying = true;
			this.Body = this.CreateSimpleBody(2, 3, 12, 9, BodyDef.BodyType.DynamicBody, false);
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
				this.Animation = Hurt;
			} else if (this.State.Equals("run")) {
				this.Animation = Run;
			} else if (this.State.Equals("up")) {
				this.Animation = Up;
			} else if (this.State.Equals("down")) {
				this.Animation = Down;
			} else {
				this.Animation = Idle;
			}


			this.RenderWithOutline(this.Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is SolidProp || Entity is RollingSpike) {
				return false;
			} 

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (!Freezed && !(State.Equals("up") || State.Equals("down") || State.Equals("idle"))) {
				Animation.Update(Dt);
			} 

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

		public Treeman() {
			_Init();
		}
	}
}
