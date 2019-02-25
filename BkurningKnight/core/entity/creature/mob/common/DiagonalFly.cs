using BurningKnight.core.entity.creature.player;
using BurningKnight.core.entity.level.entities;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature.mob.common {
	public class DiagonalFly : Fly {
		protected void _Init() {
			{
				HpMax = 5;
				IgnoreVel = true;
				Mul = 1;
			}
		}

		public static Animation Animations = Animation.Make("actor-fly", "-brown");
		protected bool Stop;
		private Vector2 LastVel;

		public Animation GetAnimation() {
			return Animations;
		}

		public override float GetWeight() {
			return 0.5f;
		}

		protected override Void CreateBody() {
			Body = World.CreateSimpleBody(this, 3, 3, W - 6, H - 6, BodyDef.BodyType.DynamicBody, false);
			Body.GetFixtureList().Get(0).SetRestitution(1f);
			Body.SetTransform(X, Y, 0);
			float F = 32;
			this.Velocity = new Point(F * (Random.Chance(50) ? -1 : 1), F * (Random.Chance(50) ? -1 : 1));
			Body.SetLinearVelocity(this.Velocity);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if (this.Body != null) {
				this.Velocity.X = this.Body.GetLinearVelocity().X;
				this.Velocity.Y = this.Body.GetLinearVelocity().Y;

				if (LastVel == null && Stop) {
					LastVel = new Vector2(Velocity.X, Velocity.Y);
				} else if (!Stop && LastVel != null) {
					Velocity.X = LastVel.X;
					Velocity.Y = LastVel.Y;
					LastVel = null;
				} 

				if (Stop) {
					this.Body.SetLinearVelocity(0, 0);
				} else {
					float A = (float) Math.Atan2(this.Velocity.Y, this.Velocity.X);
					this.Body.SetLinearVelocity(((float) Math.Cos(A)) * 32 * Mob.SpeedMod + Knockback.X * 0.2f, ((float) Math.Sin(A)) * 32 * Mob.SpeedMod + Knockback.Y * 0.2f);
				}

			} 

			Common();
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			return Entity == null || Entity is Player || Entity is Door;
		}

		public DiagonalFly() {
			_Init();
		}
	}
}
