using BurningKnight.entity.component;
using Lens.entity.component.logic;

namespace BurningKnight.entity.door {
	public class ConditionLock : Lock {
		private bool first;
		
		public ConditionLock() {
			LockedByDefault = false;
		}
		
		protected override bool Disposable() {
			return false;
		}

		public override bool Interactable() {
			return false;
		}
		
		public override void Update(float dt) {
			base.Update(dt);
			UpdateState();
		}
		
		protected virtual void UpdateState() {
			var shouldLock = ((ConditionDoor) GetComponent<OwnerComponent>().Owner).ShouldLock();

			if (shouldLock && !IsLocked) {
				SetLocked(true, null);

				if (first) {
					GetComponent<StateComponent>().Become<IdleState>();
				} else {
					GetComponent<StateComponent>().Become<ClosingState>();
				}
			} else if (!shouldLock && IsLocked) {
				SetLocked(false, null);

				if (first) {
					GetComponent<StateComponent>().Become<OpenState>();
				} else {
					GetComponent<StateComponent>().Become<OpeningState>();
				}
			}

			first = false;
		}
	}
}