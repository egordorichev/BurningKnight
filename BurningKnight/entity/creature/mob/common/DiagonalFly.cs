using BurningKnight.entity.creature.player;
using BurningKnight.entity.level.entities;
using BurningKnight.physics;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.creature.mob.common {
	public class DiagonalFly : Fly {
		public static Animation Animations = Animation.Make("actor-fly", "-brown");
		private Vector2 LastVel;
		protected bool Stop;

		public DiagonalFly() {
			_Init();
		}

		protected void _Init() {
			{
				HpMax = 5;
				IgnoreVel = true;
				Mul = 1;
			}
		}

		public Animation GetAnimation() {
			return Animations;
		}

		public override float GetWeight() {
			return 0.5f;
		}

		protected override void CreateBody() {
			Body = World.CreateSimpleBody(this, 3, 3, W - 6, H - 6, BodyDef.BodyType.DynamicBody, false);
			Body.GetFixtureList().Get(0).SetRestitution(1f);
			Body.SetTransform(X, Y, 0);
			float F = 32;
			Velocity = new Point(F * (Random.Chance(50) ? -1 : 1), F * (Random.Chance(50) ? -1 : 1));
			Body.SetLinearVelocity(Velocity);
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (Body != null) {
				Velocity.X = Body.GetLinearVelocity().X;
				Velocity.Y = Body.GetLinearVelocity().Y;

				if (LastVel == null && Stop) {
					LastVel = new Vector2(Velocity.X, Velocity.Y);
				}
				else if (!Stop && LastVel != null) {
					Velocity.X = LastVel.X;
					Velocity.Y = LastVel.Y;
					LastVel = null;
				}

				if (Stop) {
					Body.SetLinearVelocity(0, 0);
				}
				else {
					var A = (float) Math.Atan2(Velocity.Y, Velocity.X);
					Body.SetLinearVelocity((float) Math.Cos(A) * 32 * SpeedMod + Knockback.X * 0.2f, (float) Math.Sin(A) * 32 * SpeedMod + Knockback.Y * 0.2f);
				}
			}

			Common();
		}

		public override bool ShouldCollide(Object Entity, Contact Contact, Fixture Fixture) {
			return Entity == null || Entity is Player || Entity is Door;
		}
	}
}