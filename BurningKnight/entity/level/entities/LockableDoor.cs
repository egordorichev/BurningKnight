using BurningKnight.entity.component;
using BurningKnight.physics;
using Lens.entity;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.level.entities {
	public class LockableDoor : Door, CollisionFilterEntity {
		public override void PostInit() {
			base.PostInit();
			
			AddComponent(new LockComponent(this, CreateLock(), new Vector2(0, FacingSide ? 0 : 3)));
			AddComponent(new DoorBodyComponent(0, 0, Width, Height, BodyType.Static, true));
		}

		protected virtual Lock CreateLock() {
			return new IronLock();
		}

		public bool ShouldCollide(Entity entity) {
			return !(entity is Door);
		}
	}
}