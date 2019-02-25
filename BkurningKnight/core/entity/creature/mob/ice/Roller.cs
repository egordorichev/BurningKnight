using BurningKnight.core.assets;
using BurningKnight.core.entity.level;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob.ice {
	public class Roller : Mob {
		protected void _Init() {
			{
				HpMax = 16;
				Idle = GetAnimation().Get("idle");
				Hurt = GetAnimation().Get("hurt");
				Killed = GetAnimation().Get("dead");
				Roll = GetAnimation().Get("roll");
				Animation = Idle;
				W = 20;
				H = 20;
			}
		}

		public class RollerState : Mob.State<Roller>  {

		}

		public class IdleState : RollerState {
			public override Void Update(float Dt) {
				CheckForPlayer();

				if (Self.Target != null && Self.Target.Room == Self.Room) {
					Self.Become(Random.Chance(50) ? "roll" : "wait");
				} 
			}
		}

		public class WaitState : RollerState {
			private float Delay;

			public override Void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(1f, 6f);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (T >= Delay) {
					Self.Become("roll");
				} 
			}
		}

		public class RollState : RollerState {
			public override Void OnEnter() {
				base.OnEnter();
				float Angle = Random.NewAngle();
				float F = Random.NewFloat(200, 300);
				Self.Velocity.X = (float) (Math.Cos(Angle) * F);
				Self.Velocity.Y = (float) (Math.Sin(Angle) * F);
				Self.Body.SetLinearVelocity(Self.Velocity);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Self.Velocity.Len() < 30f) {
					Self.Become("wait");
				} 
			}
		}

		public static Animation Animations = Animation.Make("actor-rolling-snowball", "-white");
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Hurt;
		private AnimationData Roll;
		private AnimationData Animation;

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void Init() {
			base.Init();
			this.Body = World.CreateCircleBody(this, 0, 0, 10, BodyDef.BodyType.DynamicBody, false, 0.9f);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is Level) {
				return true;
			} 

			return base.ShouldCollide(Entity, Contact, Fixture);
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
			} else if (this.State.Equals("roll")) {
				this.Animation = Roll;
			} else {
				this.Animation = Idle;
			}


			this.RenderWithOutline(this.Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (this.Body != null) {
				this.Velocity.X = this.Body.GetLinearVelocity().X;
				this.Velocity.Y = this.Body.GetLinearVelocity().Y;
				float M = Dt * (State.Equals("roll") ? 0.3f : 8f);
				Velocity.X -= Velocity.X * M;
				Velocity.Y -= Velocity.Y * M;
				float A = (float) Math.Atan2(this.Velocity.Y, this.Velocity.X);
				this.Body.SetLinearVelocity(((float) Math.Cos(A)) * 32 * Mob.SpeedMod + Knockback.X * 0.2f, ((float) Math.Sin(A)) * 32 * Mob.SpeedMod + Knockback.Y * 0.2f);
			} 

			if (State.Equals("roll")) {
				Animation.Update(Dt * (Velocity.Len() / 80f + 0.2f));
			} else {
				Animation.Update(Dt);
			}


			Common();
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
				case "wait": {
					return new WaitState();
				}

				case "roll": {
					return new RollState();
				}

				case "idle": 
				case "roam": 
				case "alerted": {
					return new IdleState();
				}
			}

			return base.GetAi(State);
		}

		public Roller() {
			_Init();
		}
	}
}
