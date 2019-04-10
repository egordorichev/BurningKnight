using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.physics;
using Lens.entity;
using Lens.entity.component.logic;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.door {
	public class LockableDoor : Door, CollisionFilterEntity {
		public override void PostInit() {
			base.PostInit();
			
			AddComponent(new LockComponent(this, CreateLock(), new Vector2(0, FacingSide ? 1 : 4)));
			AddComponent(new DoorBodyComponent(0, 0, Width, Height, BodyType.Static, true));
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			var state = GetComponent<StateComponent>();

			if (state.StateInstance is OpenState && GetComponent<LockComponent>().Lock.IsLocked) {
				state.Become<ClosingState>();
			}
		}

		protected virtual Lock CreateLock() {
			return new IronLock();
		}

		public bool ShouldCollide(Entity entity) {
			return !(entity is Door);
		}

		protected override bool CanOpen() {
			return !GetComponent<LockComponent>().Lock.IsLocked;
		}
	}
}