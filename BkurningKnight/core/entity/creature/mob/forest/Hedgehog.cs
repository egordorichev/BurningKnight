using BurningKnight.core.assets;
using BurningKnight.core.entity.level;
using BurningKnight.core.entity.level.entities;
using BurningKnight.core.entity.trap;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob.forest {
	public class Hedgehog : Mob {
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

		public class HedgehogState : Mob.State<Hedgehog>  {

		}

		public class RollState : HedgehogState {
			private float C;
			private float S;

			public override Void OnEnter() {
				base.OnEnter();
				Self.Roll.SetFrame(0);
				float A = (float) (Self.GetAngleTo(Self.Target.X + 8, Self.Target.Y + 8));
				C = (float) Math.Cos(A);
				S = (float) Math.Sin(A);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				float Sp = 5000f * Dt;
				Self.Velocity.X += C * Sp;
				Self.Velocity.Y += S * Sp;

				if (T >= 6f) {
					Self.Become("uncircle");
				} else {
					Self.Roll.SetFrame((int) (T * 4f) % 4);
				}

			}
		}

		public class TiredState : HedgehogState {
			private float C;
			private float S;

			public override Void OnEnter() {
				base.OnEnter();
				float A = Random.NewFloat((float) (Math.PI * 2));
				C = (float) Math.Cos(A);
				S = (float) Math.Sin(A);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				Self.Velocity.X -= Self.Velocity.X * Dt * 4f;
				Self.Velocity.Y -= Self.Velocity.Y * Dt * 4f;
				float Sp = 400f * Dt;
				Self.Velocity.X += C * Sp;
				Self.Velocity.Y += S * Sp;

				if (T >= 3f) {
					Self.Become("circle");
				} 
			}
		}

		public class CircleState : HedgehogState {
			public override Void OnEnter() {
				base.OnEnter();
				Self.Circle.SetFrame(0);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				float T = this.T * 0.5f;

				if (T >= 0.9f) {
					Self.Become("roll");
				} else {
					Self.Circle.SetFrame((int) (T * 9f));
				}

			}
		}

		public class UncircleState : HedgehogState {
			public override Void OnEnter() {
				base.OnEnter();
				Self.Uncircle.SetFrame(0);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);
				float T = this.T * 0.5f;

				if (T >= 0.9f) {
					Self.Become("tired");
				} else {
					Self.Uncircle.SetFrame((int) (T * 9f));
				}

			}
		}

		public class IdleState : HedgehogState {
			public override Void Update(float Dt) {
				base.Update(Dt);
				CheckForPlayer();

				if (Self.Target != null && Self.Target.Room == Self.Room) {
					Self.Become("circle");
				} 
			}
		}

		public static Animation Animations = Animation.Make("actor-hedgehog", "-gray");
		private AnimationData Idle;
		private AnimationData Roll;
		private AnimationData Circle;
		private AnimationData Uncircle;
		private AnimationData Hurt;
		private AnimationData Killed;
		private AnimationData Animation;

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void Init() {
			base.Init();
			this.Body = this.CreateSimpleBody(2, 1, 10, 9, BodyDef.BodyType.DynamicBody, false);
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
			} else if (State.Equals("roll")) {
				this.Animation = Roll;
			} else if (State.Equals("circle")) {
				this.Animation = Circle;
			} else if (State.Equals("uncircle")) {
				this.Animation = Uncircle;
			} else {
				this.Animation = Idle;
			}


			this.RenderWithOutline(this.Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is Level || Entity == null) {
				return true;
			} 

			if (Entity is SolidProp || Entity is RollingSpike) {
				return false;
			} 

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		public override Void OnCollision(Entity Entity) {
			if (Entity is Level || Entity == null) {
				if (State.Equals("roll") && Body != null) {
					Body.SetLinearVelocity(new Vector2());
					Become("uncircle");
				} 
			} 

			base.OnCollision(Entity);
		}

		public override Void RenderShadow() {
			Graphics.ShadowSized(this.X, this.Y + 3, this.W, this.H, 0);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (!Freezed && (State.Equals("idle") || State.Equals("tired"))) {
				Animation.Update(Dt);
			} 

			if (Target != null && Target.Room != Room) {
				Become("idle");
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

		public Hedgehog() {
			_Init();
		}
	}
}
