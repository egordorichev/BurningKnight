using BurningKnight.core.entity.creature;
using BurningKnight.core.entity.creature.buff;
using BurningKnight.core.entity.level.entities;
using BurningKnight.core.physics;

namespace BurningKnight.core.entity.fx {
	public class FireFxPhysic : FireFx {
		private Body Body;

		public override Void Init() {
			base.Init();
			Body = World.CreateCircleCentredBody(this, 0, 0, 3, BodyDef.BodyType.DynamicBody, true);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);
			World.CheckLocked(this.Body).SetTransform(this.X, this.Y, 0);
		}

		public override Void Destroy() {
			base.Destroy();
			Body = World.RemoveBody(this.Body);
		}

		public override Void OnCollision(Entity Entity) {
			base.OnCollision(Entity);

			if (Entity is Creature) {
				((Creature) Entity).AddBuff(new BurningBuff());
			} else if (Entity is Door) {
				((Door) Entity).Burning = true;
			} 
		}
	}
}
