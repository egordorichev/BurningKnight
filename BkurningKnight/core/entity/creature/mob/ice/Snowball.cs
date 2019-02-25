using BurningKnight.core.assets;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob.ice {
	public class Snowball : Mob {
		protected void _Init() {
			{
				HpMax = 16;
				Idle = GetAnimation().Get("idle");
				Hurt = GetAnimation().Get("hurt");
				Killed = GetAnimation().Get("dead");
				Animation = Idle;
				W = 14;
			}
		}

		public class WaitState : Mob.State<Snowball>  {
			private float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(0.1f, 2f);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (T >= Delay) {
					Self.Become("attack");
				} 
			}
		}

		public class AttackState : Mob.State<Snowball>  {
			private Vector2 Dir;
			private float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(0.4f, 1.5f);
				float Angle;

				if (Random.Chance(40) || Self.Target == null || Self.Target.Room != Self.Room) {
					Angle = Random.NewFloat((float) (Math.PI * 2));
				} else {
					Angle = Self.GetAngleTo(Self.Target.X + 8, Self.Target.Y + 8);
				}


				float F = Random.NewFloat(90f, 150f);
				Dir = new Vector2((float) Math.Cos(Angle) * F, (float) Math.Sin(Angle) * F);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				Self.Velocity.X += Dir.X * Dt * 10;
				Self.Velocity.Y += Dir.Y * Dt * 10;

				if (T >= Delay) {
					Self.Become("idle");
				} 
			}
		}

		public static Animation Animations = Animation.Make("actor-snowball", "-white");
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Hurt;
		private AnimationData Animation;

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void Init() {
			base.Init();
			Idle.Randomize();
			this.Body = this.CreateSimpleBody(2, 0, 10, 10, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
		}

		public override Void RenderShadow() {
			Graphics.Shadow(X, Y + 3, W, H, 6);
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
			} else {
				this.Animation = Idle;
			}


			this.RenderWithOutline(this.Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override float GetWeight() {
			return 0.2f;
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

		protected override Mob.State GetAi(string State) {
			switch (State) {
				case "idle": 
				case "roam": 
				case "alerted": {
					return new WaitState();
				}

				case "attack": {
					return new AttackState();
				}
			}

			return base.GetAi(State);
		}

		public Snowball() {
			_Init();
		}
	}
}
