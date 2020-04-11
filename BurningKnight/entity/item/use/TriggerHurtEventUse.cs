using BurningKnight.entity.events;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class TriggerHurtEventUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			entity.HandleEvent(new HealthModifiedEvent {
				Amount = -1,
				Who = entity,
				From = item
			});
			
			entity.HandleEvent(new PostHealthModifiedEvent {
				Amount = -1,
				Who = entity,
				From = item
			});
		}
	}
}