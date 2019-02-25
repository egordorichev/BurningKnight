using BurningKnight.entity.level;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.ice {
	public class Roller : Mob {
		public static Animation Animations = Animation.Make("actor-rolling-snowball", "-white");
		private AnimationData Animation;
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Roll;

		public Roller() {
			_Init();
		}

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

		public Animation GetAnimation() {
			return Animations;
		}

		public override void Init() {
			base.Init();
			Body = World.CreateCircleBody(this, 0, 0, 10, BodyDef.BodyType.DynamicBody, false, 0.9f);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is Level) return true;

			return base.ShouldCollide(Entity, Contact, Fixture);
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
			else if (State.Equals("roll"))
				Animation = Roll;
			else
				Animation = Idle;


			this.RenderWithOutline(Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (Body != null) {
				Velocity.X = Body.GetLinearVelocity().X;
				Velocity.Y = Body.GetLinearVelocity().Y;
				var M = Dt * (State.Equals("roll") ? 0.3f : 8f);
				Velocity.X -= Velocity.X * M;
				Velocity.Y -= Velocity.Y * M;
				var A = (float) Math.Atan2(Velocity.Y, Velocity.X);
				Body.SetLinearVelocity((float) Math.Cos(A) * 32 * SpeedMod + Knockback.X * 0.2f, (float) Math.Sin(A) * 32 * SpeedMod + Knockback.Y * 0.2f);
			}

			if (State.Equals("roll"))
				Animation.Update(Dt * (Velocity.Len() / 80f + 0.2f));
			else
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

		public class RollerState : State<Roller> {
		}

		public class IdleState : RollerState {
			public override void Update(float Dt) {
				CheckForPlayer();

				if (Self.Target != null && Self.Target.Room == Self.Room) Self.Become(Random.Chance(50) ? "roll" : "wait");
			}
		}

		public class WaitState : RollerState {
			private float Delay;

			public override void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(1f, 6f);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (T >= Delay) Self.Become("roll");
			}
		}

		public class RollState : RollerState {
			public override void OnEnter() {
				base.OnEnter();
				var Angle = Random.NewAngle();
				var F = Random.NewFloat(200, 300);
				Self.Velocity.X = Math.Cos(Angle) * F;
				Self.Velocity.Y = Math.Sin(Angle) * F;
				Self.Body.SetLinearVelocity(Self.Velocity);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (Self.Velocity.Len() < 30f) Self.Become("wait");
			}
		}
	}
}