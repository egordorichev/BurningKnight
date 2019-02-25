using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.buff;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob.common {
	public class BurningMan : Mob {
		protected void _Init() {
			{
				HpMax = 6;
				Run = GetAnimation().Get("run");
				Hurt = GetAnimation().Get("hurt");
				Killed = GetAnimation().Get("death");
				Animation = Run;
			}
		}

		public class RunningState : Mob.State<BurningMan>  {
			public override Void OnEnter() {
				base.OnEnter();
				T = Random.NewFloat(10);
			}

			public override Void Update(float Dt) {
				base.Update(Dt);

				if (Self.Target == null) {
					return;
				} 

				MoveTo(Self.Target, 20f, 8f);
			}
		}

		public static Animation Animations = Animation.Make("actor-burningman", "-gray");
		private AnimationData Run;
		private AnimationData Hurt;
		private AnimationData Killed;
		private AnimationData Animation;

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void Init() {
			base.Init();
			this.Body = this.CreateSimpleBody(4, 3, 8, 9, BodyDef.BodyType.DynamicBody, false);
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
			} else {
				this.Animation = Run;
			}


			this.RenderWithOutline(this.Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (!Freezed) {
				Animation.Update(Dt);
			} 

			if (this.Target != null && this.Target.Room == this.Room && !this.HasBuff(Buffs.BURNING)) {
				BurningBuff BurningBuff = new BurningBuff();
				BurningBuff.Infinite = true;
				AddBuff(BurningBuff);
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
			return new RunningState();
		}

		public BurningMan() {
			_Init();
		}
	}
}
