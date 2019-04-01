using BurningKnight.entity.item;

namespace BurningKnight.entity.component {
	public class LampComponent : ItemComponent {
		public override void Set(Item item) {
			if (Item is Lamp lamp) {
				lamp.Unequip(Entity);
			}
			
			base.Set(item);
			
			if (Item is Lamp l) {
				l.Equip(Entity);
				Entity.Area.Add(item);
			}
		}

		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Lamp;
		}
	}
}