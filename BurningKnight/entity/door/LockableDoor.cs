using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.physics;
using Lens.entity;
using Lens.entity.component.logic;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.door {
	public class LockableDoor : Door, CollisionFilterEntity {
		protected bool SkipLock;
		protected bool Replaced;

		protected virtual Rectangle GetHitbox() {
			return new Rectangle(0, Vertical ? 0 : 5, (int) Width, Vertical ? (int) Height + 8 : 7);
		}

		protected virtual Vector2 GetLockOffset() {
			return Vertical ? new Vector2(0, 1) : new Vector2(0, 2);
		}
		
		public override void PostInit() {
			base.PostInit();

			if (!SkipLock) {
				AddLock(Replaced ? new IronLock() : CreateLock());
			}
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (SkipLock) {
				return;
			}
			
			var state = GetComponent<StateComponent>();

			if (TryGetComponent<LockComponent>(out var l)) {
				if (l.Lock == null || l.Lock.Done) {
					ReplaceLock();
				} else if ((state.StateInstance is OpenState || state.StateInstance is OpeningState) && l.Lock.IsLocked) {
					state.Become<ClosingState>();
				} else if (OpenByDefault && (state.StateInstance is ClosedState || state.StateInstance is ClosingState) && !l.Lock.IsLocked) {
					state.Become<ClosingState>();
				}
			} else {
				ReplaceLock();
			}
		}

		private void ReplaceLock() {
			Replaced = true;
			AddLock(new IronLock());
		}

		private void AddLock(Lock l) {
			if (l == null) {
				return;
			}
			
			if (HasComponent<LockComponent>()) {
				RemoveComponent<LockComponent>();
			}
			
			AddComponent(new LockComponent(this, l, GetLockOffset()));

			if (!HasComponent<DoorBodyComponent>()) {
				var box = GetHitbox();

				if (this is CustomDoor && !Vertical) {
					box.Y += 8;
				}
				
				AddComponent(new DoorBodyComponent(box.X, box.Y, box.Width, box.Height, BodyType.Static, true));
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			SkipLock = stream.ReadBoolean();
			Replaced = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(SkipLock || !TryGetComponent<LockComponent>(out var l) || l.Lock == null || l.Lock.Done);
			stream.WriteBoolean(Replaced);
		}

		protected virtual Lock CreateLock() {
			return new IronLock();
		}

		public bool ShouldCollide(Entity entity) {
			return !(entity is Door);
		}

		protected override bool CanOpen() {
			return !TryGetComponent<LockComponent>(out var c) || !c.Lock.IsLocked;
		}

		public override void Render() {
			base.Render();

			if (!SkipLock && TryGetComponent<LockComponent>(out var l)) {
				l.Lock?.RealRender();
			}
		}
	}
}