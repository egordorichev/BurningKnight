using BurningKnight.core.entity.creature.mob.common;
using BurningKnight.core.entity.level;
using BurningKnight.core.physics;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.creature.mob.ice {
	public class SnowballFly : DiagonalFly {
		public static Animation Animations = Animation.Make("actor-fly", "-snow");

		public Animation GetAnimation() {
			return Animations;
		}

		protected override Void CreateBody() {
			W = 23;
			H = 16;
			Body = World.CreateSimpleBody(this, 6, 2, W - 12, H - 8, BodyDef.BodyType.DynamicBody, false);
			Body.GetFixtureList().Get(0).SetRestitution(1f);
			Body.SetTransform(X, Y, 0);
			float F = 32;
			this.Velocity = new Point(F * (Random.Chance(50) ? -1 : 1), F * (Random.Chance(50) ? -1 : 1));
			Body.SetLinearVelocity(this.Velocity);
		}

		public override Void DeathEffects() {
			base.DeathEffects();

			if (Touches[Terrain.FLOOR_A] || Touches[Terrain.FLOOR_B] || Touches[Terrain.FLOOR_C] || Touches[Terrain.FLOOR_D]) {
				Snowball Snowball = new Snowball();
				Snowball.X = this.X;
				Snowball.Y = this.Y;
				Dungeon.Area.Add(Snowball.Add());
			} 
		}

		protected override List GetDrops<Item> () {
			return new List<>();
		}
	}
}
