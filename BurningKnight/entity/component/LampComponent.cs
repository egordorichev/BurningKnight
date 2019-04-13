using BurningKnight.entity.item;
using Lens.util;

namespace BurningKnight.entity.component {
	public class LampComponent : ItemComponent {
		public override void Set(Item item) {
			if (Item != null) {
				Entity.Area.Remove(Item);
			}

			base.Set(item);

			Item.Use(Entity);
			Entity.Area.Add(item);
			
			Log.Error("Picked up lamp " + item.Id);
		}

		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Lamp;
		}
	}
}