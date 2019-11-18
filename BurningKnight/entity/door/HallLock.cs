using BurningKnight.entity.creature.npc;
using Lens.entity.component.logic;

namespace BurningKnight.entity.door {
	public class HallLock : IronLock {
		protected override void UpdateState() {
			var found = false;

			foreach (var n in Area.Tagged[Tags.Npc]) {
				if (n is ShopNpc sn && !sn.Hidden) {
					found = true;
					break;
				}
			}

			if (!found && !IsLocked) {
				SetLocked(true, null);
				GetComponent<StateComponent>().Become<ClosingState>();
			} else if (found && IsLocked) {
				SetLocked(false, null);
				GetComponent<StateComponent>().Become<OpeningState>();
			}
		}
	}
}