using BurningKnight.entity.creature;
using BurningKnight.entity.events;
using BurningKnight.entity.item.use.parent;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class DoOnEnemyCollisionUse : DoUsesUse {
		protected override void DoAction(Entity entity, Item item, ItemUse use) {
			
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent ev && ev.Entity is Creature c && !c.IsFriendly()) {
				foreach (var u in Uses) {
					u.Use(c, Item);
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}