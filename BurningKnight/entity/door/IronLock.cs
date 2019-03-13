using BurningKnight.entity.component;
using Lens.entity.component.logic;

namespace BurningKnight.entity.door {
	public class IronLock : Lock {
		protected override bool Disposable() {
			return false;
		}

		public override void Update(float dt) {
			base.Update(dt);

			var shouldLock = false;

			foreach (var player in Area.Tags[Tags.Player]) {
				if (player.GetComponent<RoomComponent>().Room.Tagged[Tags.Mob].Count > 0) {
					shouldLock = true;
					break;
				}
			}

			if (shouldLock && !IsLocked) {
				IsLocked = true;
				GetComponent<StateComponent>().Become<ClosingState>();
			} else if (!shouldLock && IsLocked) {
				IsLocked = false;
				GetComponent<StateComponent>().Become<OpeningState>();
			}
		}
	}
}