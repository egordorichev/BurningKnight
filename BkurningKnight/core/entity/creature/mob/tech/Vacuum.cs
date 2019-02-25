using BurningKnight.core.assets;
using BurningKnight.core.entity.creature.player;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature.mob.tech {
	public class Vacuum : Bot {
		protected void _Init() {
			{
				HpMax = 16;
				Idle = GetAnimation().Get("idle");
				Hurt = GetAnimation().Get("hurt");
				Killed = GetAnimation().Get("dead");
				Animation = Idle;
			}
		}

		private class SuckParticle {
			public float T;
			public float A;
			public float S;
			public float An;
			public float Sp;

			public SuckParticle() {
				A = Random.NewAngle();
				S = Random.NewFloat(1, 5);
				An = Random.NewFloat(360);
				Sp = Random.NewFloat(0.5f, 1.5f);
			}
		}

		public class IdleState : Mob.State<Vacuum>  {

		}

		public static Animation Animations = Animation.Make("actor-vacum", "-normal");
		private AnimationData Idle;
		private AnimationData Killed;
		private AnimationData Hurt;
		private AnimationData Animation;
		private float Tm;
		private List<SuckParticle> Parts = new List<>();

		public Animation GetAnimation() {
			return Animations;
		}

		public override Void KnockBackFrom(Entity From, float Force) {

		}

		public override Void Init() {
			base.Init();
			W = 14;
			H = 11;
			Body = World.CreateSimpleBody(this, 2, 2, W - 4, H - 4, BodyDef.BodyType.DynamicBody, false);
			Body.GetFixtureList().Get(0).SetRestitution(1f);
			Body.SetTransform(X, Y, 0);
			float F = 20;
			this.Velocity = new Point(F * (Random.Chance(50) ? -1 : 1), F * (Random.Chance(50) ? -1 : 1));
			Body.SetLinearVelocity(this.Velocity);
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
				this.Animation = Idle;
			}


			Graphics.StartAlphaShape();
			float Dt = Dungeon.Game.GetState().IsPaused() ? 0 : Gdx.Graphics.GetDeltaTime();

			for (int I = Parts.Size() - 1; I >= 0; I--) {
				SuckParticle P = Parts.Get(I);
				P.T += Dt;
				Graphics.Shape.SetColor(1, 1, 1, Math.Min(1, P.T * 2f));
				float D = 24 - P.T * P.S * 8;

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
			this.RenderWithOutline(this.Animation);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			base.RenderStats();
		}

		public override Void RenderShadow() {
			Graphics.Shadow(X + 1, Y + 2, W - 1, H, 0);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (this.Body != null) {
				this.Velocity.X = this.Body.GetLinearVelocity().X;
				this.Velocity.Y = this.Body.GetLinearVelocity().Y;
				float A = (float) Math.Atan2(this.Velocity.Y, this.Velocity.X);
				this.Body.SetLinearVelocity(((float) Math.Cos(A)) * 20 * Mob.SpeedMod, ((float) Math.Sin(A)) * 20 * Mob.SpeedMod);
			} 

			if (Player.Instance.Room == Room) {
				this.Suck(Player.Instance, Dt);
			} 

			Animation.Update(Dt);
			base.Common();
		}

		private Void Suck(Creature Creature, float Dt) {
			float Dx = Creature.X + Creature.W / 2 - this.X - this.W / 2;
			float Dy = Creature.Y + Creature.H / 2 - this.Y - this.H / 2;
			float D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);
			float Max = 64;

			if (D < Max) {
				float Mod = (64 - D) * 20 * Dt;
				Creature.Velocity.X -= Dx / D * (Mod);
				Creature.Velocity.Y -= Dy / D * (Mod);
			} 
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

		protected override Mob.State GetAi(string State) {
			return new IdleState();
		}

		public Vacuum() {
			_Init();
		}
	}
}
