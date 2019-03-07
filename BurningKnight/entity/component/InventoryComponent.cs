using System.Collections.Generic;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using Lens.entity.component;
using Lens.util.file;

namespace BurningKnight.entity.component {
	public class InventoryComponent : SaveableComponent {
		public List<Item> Items = new List<Item>();

		public void Pickup(Item item) {
			if (!Send(new ItemCheckEvent {
				Item = item
			})) {
				item.Use(Entity);
				Add(item);
			}
		}
		
		public void Add(Item item) {
			Items.Add(item);
			Entity.Area.Remove(item);

			item.RemoveDroppedComponents();
			item.AddComponent(new OwnerComponent(Entity));

			var e = new ItemAddedEvent {
				Item = item
			};
			
			Send(e);
			item.HandleEvent(e);
		}

		public void Remove(Item item) {
			Items.Remove(item);

			var e = new ItemRemovedEvent {
				Item = item
			};
			
			Send(e);
			item.HandleEvent(e);
			
			item.Center = Entity.Center;
			Entity.Area.Add(item);
			item.AddDroppedComponents();
			item.RemoveComponent<OwnerComponent>();
		}

		public override void Update(float dt) {
			base.Update(dt);

			for (int i = Items.Count - 1; i >= 0; i--) {
				var item = Items[i];

				if (item.Done) {
					Remove(item);
				}
			}
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			// todo: save
		}

		public override void Load(FileReader reader) {
			base.Load(reader);
			// todo: load
		}
	}
}