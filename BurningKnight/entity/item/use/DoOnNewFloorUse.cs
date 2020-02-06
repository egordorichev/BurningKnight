using BurningKnight.entity.events;
using BurningKnight.entity.item.use.parent;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class DoOnNewFloorUse : DoUsesUse {
		private Entity e;
		
		protected override void DoAction(Entity entity, Item item, ItemUse use) {
			e = entity;
		}

		public override bool HandleEvent(Event ev) {
			if (ev is NewFloorEvent) {
				foreach (var u in Uses) {
					u.Use(e, Item);
				}
			}
			
			return base.HandleEvent(ev);
		}
	}
}