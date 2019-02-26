using BurningKnight.entity.creature;
using BurningKnight.entity.creature.buff;
using BurningKnight.entity.level.entities;
using BurningKnight.physics;

namespace BurningKnight.entity.fx {
	public class FireFxPhysic : FireFx {
		private Body Body;

		public override void Init() {
			base.Init();
			Body = World.CreateCircleCentredBody(this, 0, 0, 3, BodyDef.BodyType.DynamicBody, true);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
		}

		public override void Update(float Dt) {
			base.Update(Dt);
			World.CheckLocked(Body).SetTransform(this.X, this.Y, 0);
		}

		public override void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(Body);
		}

		public override void OnCollision(Entity Entity) {
			base.OnCollision(Entity);

			if (Entity is Creature)
				((Creature) Entity).AddBuff(new BurningBuff());
			else if (Entity is Door) ((Door) Entity).Burning = true;
		}
	}
}