using BurningKnight.entity.creature.player;
using BurningKnight.physics;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.creature.mob.tech {
	public class Vacuum : Bot {
		public static Animation Animations = Animation.Make("actor-vacum", "-normal");
		private AnimationData Animation;
		private AnimationData Hurt;
		private AnimationData Idle;
		private AnimationData Killed;
		private List<SuckParticle> Parts = new List<>();
		private float Tm;

		public Vacuum() {
			_Init();
		}

		protected void _Init() {
			{
				HpMax = 16;
				Idle = GetAnimation().Get("idle");
				Hurt = GetAnimation().Get("hurt");
				Killed = GetAnimation().Get("dead");
				Animation = Idle;
			}
		}

		public Animation GetAnimation() {
			return Animations;
		}

		public override void KnockBackFrom(Entity From, float Force) {
		}

		public override void Init() {
			base.Init();
			W = 14;
			H = 11;
			Body = World.CreateSimpleBody(this, 2, 2, W - 4, H - 4, BodyDef.BodyType.DynamicBody, false);
			Body.GetFixtureList().Get(0).SetRestitution(1f);
			Body.SetTransform(X, Y, 0);
			float F = 20;
			Velocity = new Point(F * (Random.Chance(50) ? -1 : 1), F * (Random.Chance(50) ? -1 : 1));
			Body.SetLinearVelocity(Velocity);
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


			Graphics.StartAlphaShape();
			float Dt = Dungeon.Game.GetState().IsPaused() ? 0 : Gdx.Graphics.GetDeltaTime();

			for (var I = Parts.Size() - 1; I >= 0; I--) {
				SuckParticle P = Parts.Get(I);
				P.T += Dt;
				Graphics.Shape.SetColor(1, 1, 1, Math.Min(1, P.T * 2f));
				var D = 24 - P.T * P.S * 8;

				if (D <= 2f) {
					Parts.Remove(I);

					continue;
				}

				Graphics.Shape.Rect(this.X + W / 2 + (float) Math.Cos(P.A) * D, this.Y + H / 2 + (float) Math.Sin(P.A) * D, P.S / 2, P.S / 2, P.S, P.S, 1, 1, P.An);
			}

			Tm += Dt;

			if (Tm >= 0.1f) {
				Tm = 0;
				Parts.Add(new SuckParticle());
			}

			Graphics.EndAlphaShape();
			this.RenderWithOutline(Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override void RenderShadow() {
			Graphics.Shadow(X + 1, Y + 2, W - 1, H, 0);
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (Body != null) {
				Velocity.X = Body.GetLinearVelocity().X;
				Velocity.Y = Body.GetLinearVelocity().Y;
				var A = (float) Math.Atan2(Velocity.Y, Velocity.X);
				Body.SetLinearVelocity((float) Math.Cos(A) * 20 * SpeedMod, (float) Math.Sin(A) * 20 * SpeedMod);
			}

			if (Player.Instance.Room == Room) Suck(Player.Instance, Dt);

			Animation.Update(Dt);
			Common();
		}

		private void Suck(Creature Creature, float Dt) {
			var Dx = Creature.X + Creature.W / 2 - this.X - W / 2;
			var Dy = Creature.Y + Creature.H / 2 - this.Y - H / 2;
			var D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);
			float Max = 64;

			if (D < Max) {
				var Mod = (64 - D) * 20 * Dt;
				Creature.Velocity.X -= Dx / D * Mod;
				Creature.Velocity.Y -= Dy / D * Mod;
			}
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
			return new IdleState();
		}

		private class SuckParticle {
			public float A;
			public float An;
			public float S;
			public float Sp;
			public float T;

			public SuckParticle() {
				A = Random.NewAngle();
				S = Random.NewFloat(1, 5);
				An = Random.NewFloat(360);
				Sp = Random.NewFloat(0.5f, 1.5f);
			}
		}

		public class IdleState : State<Vacuum> {
		}
	}
}