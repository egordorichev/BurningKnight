using BurningKnight.entity.creature.buff;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.common {
	public class BurningMan : Mob {
		public static Animation Animations = Animation.Make("actor-burningman", "-gray");
		private AnimationData Animation;
		private AnimationData Hurt;
		private AnimationData Killed;
		private AnimationData Run;

		public BurningMan() {
			_Init();
		}

		protected void _Init() {
			{
				HpMax = 6;
				Run = GetAnimation().Get("run");
				Hurt = GetAnimation().Get("hurt");
				Killed = GetAnimation().Get("death");
				Animation = Run;
			}
		}

		public Animation GetAnimation() {
			return Animations;
		}

		public override void Init() {
			base.Init();
			Body = CreateSimpleBody(4, 3, 8, 9, BodyDef.BodyType.DynamicBody, false);
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
			else
				Animation = Run;


			this.RenderWithOutline(Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (!Freezed) Animation.Update(Dt);

			if (Target != null && Target.Room == Room && !HasBuff(Buffs.BURNING)) {
				var BurningBuff = new BurningBuff();
				BurningBuff.Infinite = true;
				AddBuff(BurningBuff);
			}

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
			return new RunningState();
		}

		public class RunningState : State<BurningMan> {
			public override void OnEnter() {
				base.OnEnter();
				T = Random.NewFloat(10);
			}

			public override void Update(float Dt) {
				base.Update(Dt);

				if (Self.Target == null) return;

				MoveTo(Self.Target, 20f, 8f);
			}
		}
	}
}