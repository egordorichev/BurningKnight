using BurningKnight.entity.level.entities;
using BurningKnight.entity.trap;
using BurningKnight.physics;
using BurningKnight.util;

namespace BurningKnight.entity.creature.mob.common {
	public class Fly : Mob {
		public static Animation Animations = Animation.Make("actor-fly", "-normal");
		private AnimationData Animation;
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;

		public Fly() {
			_Init();
		}

		protected void _Init() {
			{
				HpMax = 1;
				Mul = 0.95f;
				Flying = true;
			}
		}

		public Animation GetAnimation() {
			return Animations;
		}

		protected override void OnHurt(int A, Entity Creature) {
			base.OnHurt(A, Creature);
			PlaySfx("damage_clown");
		}

		public override void DeathEffects() {
			base.DeathEffects();
			DeathEffect(Killed);
			PlaySfx("death_clown");
			Body.SetLinearVelocity(new Vector2());
			Poof();
		}

		public override void Init() {
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

		protected void CreateBody() {
			Body = World.CreateSimpleBody(this, 3, 3, W - 6, H - 3, BodyDef.BodyType.DynamicBody, false);
			Body.SetTransform(X, Y, 0);
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (!Freezed) Animation.Update(Dt * SpeedMod);

			Common();
		}

		public override void Render() {
			if (Invt > 0)
				Animation = Hurt;
			else
				Animation = Idle;


			RenderWithOutline(Animation);
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public override float GetWeight() {
			return 0.1f;
		}

		public override void RenderShadow() {
			Graphics.Shadow(X + 3, Y, W - 6, H + 4, 4);
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			if (Entity is RollingSpike || Entity is SolidProp) return false;

			return base.ShouldCollide(Entity, Contact, Fixture);
		}

		protected override State GetAi(string State) {
			return new IdleState();
		}

		public class IdleState : State<Fly> {
		}
	}
}