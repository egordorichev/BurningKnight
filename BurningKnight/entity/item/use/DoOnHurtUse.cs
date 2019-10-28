using BurningKnight.entity.events;
using BurningKnight.entity.item.use.parent;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class DoOnHurtUse : DoUsesUse {
		protected override void DoAction(Entity entity, Item item, ItemUse use) {
			
		}

		public override bool HandleEvent(Event e) {
			if (e is PostHealthModifiedEvent ev && ev.Amount < 0) {
				foreach (var u in Uses) {
					u.Use(ev.Who, Item);
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}