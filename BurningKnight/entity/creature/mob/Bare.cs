using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.creature.mob {
	public class Bare : Mob {
		public static Animation Animations = Animation.Make("actor-mage", "-yellow");
		private AnimationData Animation;
		private AnimationData Appear;
		private AnimationData Dissappear;
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;

		public Bare() {
			_Init();
		}

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

		public Animation GetAnimation() {
			return Animations;
		}

		public override void Init() {
			base.Init();
			Body = CreateSimpleBody(7, 0, 7, 14, BodyDef.BodyType.DynamicBody, false);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public override void Render() {
			if (Target != null) {
				Flipped = Target.X < this.X;
			}
			else {
				if (Math.Abs(Velocity.X) > 1f) Flipped = Velocity.X < 0;
			}


			if (Dead)
				Animation = Killed;
			else if (Invt > 0)
				Animation = Hurt;
			else if (State.Equals("dissappear"))
				Animation = Dissappear;
			else if (State.Equals("appear"))
				Animation = Appear;
			else
				Animation = Idle;


			this.RenderWithOutline(Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
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

		protected override State GetAi(string State) {
			switch (State) {
			}

			return base.GetAi(State);
		}
	}
}