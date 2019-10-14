using BurningKnight.entity.buff;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class GiveBuffImmunityUse : GiveBuffUse {
		public override void Use(Entity entity, Item item) {
			
		}

		public override bool HandleEvent(Event e) {
			if (e is BuffCheckEvent ev && ev.Buff.Type == Buff) {
				return true;
			}
			
			return base.HandleEvent(e);
		}
	}
}