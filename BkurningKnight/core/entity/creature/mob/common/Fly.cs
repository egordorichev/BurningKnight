using BurningKnight.core.assets;
using BurningKnight.core.entity.level.entities;
using BurningKnight.core.entity.trap;
using BurningKnight.core.physics;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.mob.common {
	public class Fly : Mob {
		protected void _Init() {
			{
				HpMax = 1;
				Mul = 0.95f;
				Flying = true;
			}
		}

		public class IdleState : Mob.State<Fly>  {

		}

		public static Animation Animations = Animation.Make("actor-fly", "-normal");
		private AnimationData Idle;
		private AnimationData Hurt;
		private AnimationData Killed;
		private AnimationData Animation;

		public Animation GetAnimation() {
			return Animations;
		}

		protected override Void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			this.PlaySfx("damage_clown");
		}

		public override Void DeathEffects() {
			base.DeathEffects();
			DeathEffect(Killed);
			this.PlaySfx("death_clown");
			Body.SetLinearVelocity(new Vector2());
			Poof();
		}

		public override Void Init() {
			base.Init();
			W = 16;
			H = 12;
			CreateBody();
			Idle = GetAnimation().Get("idle");
			Idle.Randomize();
			Animation = Idle;
			Hurt = GetAnimation().Get("hurt");
			Killed = GetAnimation().Get("dead");
		}

		protected Void CreateBody() {
			Body = World.CreateSimpleBody(this, 3, 3, W - 6, H - 3, BodyDef.BodyType.DynamicBody, false);
			Body.SetTransform(X, Y, 0);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (!Freezed) {
				Animation.Update(Dt * Mob.SpeedMod);
			} 

			Common();
		}

		public override Void Render() {
			if (Invt > 0) {
				Animation = Hurt;
			} else {
				Animation = Idle;
			}


			RenderWithOutline(Animation);
		}

		public override Void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public override float GetWeight() {
			return 0.1f;
		}

		public override Void RenderShadow() {
			Graphics.Shadow(X + 3, Y, W - 6, H + 4, 4);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is RollingSpike || Entity is SolidProp) {
				return false;
			} 

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		protected override State GetAi(string State) {
			return new IdleState();
		}

		public Fly() {
			_Init();
		}
	}
}
