using BurningKnight.core.assets;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob {
	public class Bare : Mob {
		protected void _Init() {
			{
				HpMax = 16;
				Idle = GetAnimation().Get("idle");
				Hurt = GetAnimation().Get("hurt");
				Killed = GetAnimation().Get("dead");
				Appear = GetAnimation().Get("appear");
				Dissappear = GetAnimation().Get("dissappear");
				Animation = Idle;
			}
		}

		public static Animation Animations = Animation.Make("actor-mage", "-yellow");
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Hurt;
		private AnimationData Appear;
		private AnimationData Dissappear;
		private AnimationData Animation;

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void Init() {
			base.Init();
			this.Body = this.CreateSimpleBody(7, 0, 7, 14, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
		}

		public override Void Destroy() {
			base.Destroy();
			this.Body = World.RemoveBody(this.Body);
		}

		public override Void Render() {
			if (this.Target != null) {
				this.Flipped = this.Target.X < this.X;
			} else {
				if (Math.Abs(this.Velocity.X) > 1f) {
					this.Flipped = this.Velocity.X < 0;
				} 
			}


			if (this.Dead) {
				this.Animation = Killed;
			} else if (this.Invt > 0) {
				this.Animation = Hurt;
			} else if (this.State.Equals("dissappear")) {
				this.Animation = Dissappear;
			} else if (this.State.Equals("appear")) {
				this.Animation = Appear;
			} else {
				this.Animation = Idle;
			}


			this.RenderWithOutline(this.Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
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

		protected override State GetAi(string State) {
			switch (State) {
			}

			return base.GetAi(State);
		}

		public Bare() {
			_Init();
		}
	}
}
