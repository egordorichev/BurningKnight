using BurningKnight.entity.component;
using BurningKnight.entity.creature.npc.dungeon;
using BurningKnight.util;
using Lens.entity;

namespace BurningKnight.entity.item.stand {
	public class TrashGoblinStand : ItemStand {
		protected override string GetSprite() {
			return "scourge_stand";
		}

		protected override void OnTake(Item item, Entity who) {
			base.OnTake(item, who);

			foreach (var n in GetComponent<RoomComponent>().Room.Tagged[Tags.Npc]) {
				if (n is TrashGoblin g) {
					g.Free();
					AnimationUtil.Poof(Center);
					Done = true;

					return;
				}
			}
		}
	}
}