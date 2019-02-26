using System.Collections.Generic;
using BurningKnight.entity.item;
using Lens.entity.component;

namespace BurningKnight.entity.component {
	public class InventoryComponent : Component {
		public List<Item> Items = new List<Item>();
		
		public InventoryComponent() {
			
		}

		public void Add(Item item) {
			Items.Add(item);
			Entity.Area.Remove(item);

			item.AddComponent(new OwnerComponent(Entity));
		}

		public void Remove(Item item) {
			Items.Remove(item);
			Entity.Area.Remove(item);
			
			item.RemoveComponent<OwnerComponent>();
		}

		public override void Update(float dt) {
			base.Update(dt);

			for (int i = Items.Count - 1; i >= 0; i--) {
				var item = Items[i];

				if (item.Done) {
					
				}
			}
		}
	}
}