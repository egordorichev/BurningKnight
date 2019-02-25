using BurningKnight.entity.level;
using BurningKnight.entity.level.entities;
using BurningKnight.entity.trap;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.forest {
	public class Hedgehog : Mob {
		public static Animation Animations = Animation.Make("actor-hedgehog", "-gray");
		private AnimationData Animation;
		private AnimationData Circle;
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Roll;
		private AnimationData Uncircle;

		public Hedgehog() {
			_Init();
		}

		protected void _Init() {
			{
				HpMax = 8;
				Idle = GetAnimation().Get("idle");
				Hurt = GetAnimation().Get("hurt");
				Roll = GetAnimation().Get("roll");
				Circle = GetAnimation().Get("circle");
				Killed = GetAnimation().Get("dead");
				Uncircle = GetAnimation().Get("uncircle");
				Animation = Idle;
			}
		}

		public Animation GetAnimation() {
			return Animations;
		}

		public override void Init() {
			base.Init();
			Body = CreateSimpleBody(2, 1, 10, 9, BodyDef.BodyType.DynamicBody, false);
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
			else if (State.Equals("roll"))
				Animation = Roll;
			else if (State.Equals("circle"))
				Animation = Circle;
			else if (State.Equals("uncircle"))
				Animation = Uncircle;
			else
				Animation = Idle;


			this.RenderWithOutline(Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is Level || Entity == null) return true;

			if (Entity is SolidProp || Entity is RollingSpike) return false;

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		public override void OnCollision(Entity Entity) {
			if (Entity is Level || Entity == null)
				if (State.Equals("roll") && Body != null) {
					Body.SetLinearVelocity(new Vector2());
					Become("uncircle");
				}

			base.OnCollision(Entity);
		}

		public override void RenderShadow() {
			Graphics.ShadowSized(this.X, this.Y + 3, W, H, 0);
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (!Freezed && (State.Equals("idle") || State.Equals("tired"))) Animation.Update(Dt);

			if (Target != null && Target.Room != Room) Become("idle");

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
				case "circle": {
					return new CircleState();
				}

				case "uncircle": {
					return new UncircleState();
				}

				case "roll": {
					return new RollState();
				}

				case "tired": {
					return new TiredState();
				}

				case "idle":
				case "roam":
				case "alerted": {
					return new IdleState();
				}
			}

			return base.GetAi(State);
		}

		public class HedgehogState : State<Hedgehog> {
		}

		public class RollState : HedgehogState {
			private float C;
			private float S;

			public override void OnEnter() {
				base.OnEnter();
				Self.Roll.SetFrame(0);
				var A = Self.GetAngleTo(Self.Target.X + 8, Self.Target.Y + 8);
				C = (float) Math.Cos(A);
				S = (float) Math.Sin(A);
			}

			public override void Update(float Dt) {
				base.Update(Dt);
				var Sp = 5000f * Dt;
				Self.Velocity.X += C * Sp;
				Self.Velocity.Y += S * Sp;

				if (T >= 6f)
					Self.Become("uncircle");
				else
					Self.Roll.SetFrame((int) (T * 4f) % 4);
			}
		}

		public class TiredState : HedgehogState {
			private float C;
			private float S;

			public override void OnEnter() {
				base.OnEnter();
				var A = Random.NewFloat(Math.PI * 2);
				C = (float) Math.Cos(A);
				S = (float) Math.Sin(A);
			}

			public override void Update(float Dt) {
				base.Update(Dt);
				Self.Velocity.X -= Self.Velocity.X * Dt * 4f;
				Self.Velocity.Y -= Self.Velocity.Y * Dt * 4f;
				var Sp = 400f * Dt;
				Self.Velocity.X += C * Sp;
				Self.Velocity.Y += S * Sp;

				if (T >= 3f) Self.Become("circle");
			}
		}

		public class CircleState : HedgehogState {
			public override void OnEnter() {
				base.OnEnter();
				Self.Circle.SetFrame(0);
			}

			public override void Update(float Dt) {
				base.Update(Dt);
				var T = this.T * 0.5f;

				if (T >= 0.9f)
					Self.Become("roll");
				else
					Self.Circle.SetFrame((int) (T * 9f));
			}
		}

		public class UncircleState : HedgehogState {
			public override void OnEnter() {
				base.OnEnter();
				Self.Uncircle.SetFrame(0);
			}

			public override void Update(float Dt) {
				base.Update(Dt);
				var T = this.T * 0.5f;

				if (T >= 0.9f)
					Self.Become("tired");
				else
					Self.Uncircle.SetFrame((int) (T * 9f));
			}
		}

		public class IdleState : HedgehogState {
			public override void Update(float Dt) {
				base.Update(Dt);
				CheckForPlayer();

				if (Self.Target != null && Self.Target.Room == Self.Room) Self.Become("circle");
			}
		}
	}
}