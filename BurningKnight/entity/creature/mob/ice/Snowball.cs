using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.ice {
	public class Snowball : Mob {
		public static Animation Animations = Animation.Make("actor-snowball", "-white");
		private AnimationData Animation;
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;

		public Snowball() {
			_Init();
		}

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

		public Animation GetAnimation() {
			return Animations;
		}

		public override void Init() {
			base.Init();
			Idle.Randomize();
			Body = CreateSimpleBody(2, 0, 10, 10, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
		}

		public override void RenderShadow() {
			Graphics.Shadow(X, Y + 3, W, H, 6);
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
			else
				Animation = Idle;


			this.RenderWithOutline(Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override float GetWeight() {
			return 0.2f;
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

		public class WaitState : State<Snowball> {
			private float Delay;

			public override void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(0.1f, 2f);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (T >= Delay) Self.Become("attack");
			}
		}

		public class AttackState : State<Snowball> {
			private float Delay;
			private Vector2 Dir;

			public override void OnEnter() {
				base.OnEnter();
				Delay = Random.NewFloat(0.4f, 1.5f);
				float Angle;

				if (Random.Chance(40) || Self.Target == null || Self.Target.Room != Self.Room)
					Angle = Random.NewFloat(Math.PI * 2);
				else
					Angle = Self.GetAngleTo(Self.Target.X + 8, Self.Target.Y + 8);


				var F = Random.NewFloat(90f, 150f);
				Dir = new Vector2((float) Math.Cos(Angle) * F, (float) Math.Sin(Angle) * F);
			}

			public override void Update(float Dt) {
				base.Update(Dt);
				Self.Velocity.X += Dir.X * Dt * 10;
				Self.Velocity.Y += Dir.Y * Dt * 10;

				if (T >= Delay) Self.Become("idle");
			}
		}
	}
}