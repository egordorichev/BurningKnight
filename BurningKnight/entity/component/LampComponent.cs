using BurningKnight.entity.item;

namespace BurningKnight.entity.component {
	public class LampComponent : ItemComponent {
		public override void Init() {
			base.Init();
			Set(ItemRegistry.Create("lamp", Entity.Area));
		}

		public override void Set(Item item) {
			if (Item is Lamp lamp) {
				lamp.Unequip(Entity);
			}
			
			base.Set(item);
			
			if (Item is Lamp l) {
				l.Unequip(Entity);
				Entity.Area.Add(item);
			}
		}

		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Lamp;
		}
	}
}