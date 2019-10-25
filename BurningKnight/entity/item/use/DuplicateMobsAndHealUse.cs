using BurningKnight.entity.component;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class DuplicateMobsAndHealUse : DuplicateMobsUse {
		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);
			var room = entity.GetComponent<RoomComponent>().Room;

			if (room == null || room.Tagged[Tags.Mob].Count == 0) {
				return;
			}

			entity.GetComponent<HealthComponent>().ModifyHealth(2, item);
		}
	}
}