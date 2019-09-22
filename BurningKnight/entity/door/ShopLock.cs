using BurningKnight.entity.creature.npc;
using BurningKnight.level.rooms;
using Lens.entity.component.logic;

namespace BurningKnight.entity.door {
	public class ShopLock : IronLock {
		protected override void UpdateState() {
			var found = false;

			foreach (var r in rooms) {
				if (r.Type != RoomType.Connection && r.Tagged[Tags.Npc].Count > 0) {
					foreach (var n in r.Tagged[Tags.Npc]) {
						if (n is ShopNpc sn && !sn.Hidden) {
							found = true;
							break;
						}
					}
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