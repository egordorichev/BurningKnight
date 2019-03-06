using BurningKnight.entity.events;
using BurningKnight.entity.item;
using Lens.entity;
using Lens.entity.component;
using Lens.util.file;

namespace BurningKnight.entity.component {
	public class ItemComponent : SaveableComponent {
		public Item Item { get; private set; }

		public virtual void Set(Item item) {
			if (Item != null) {
				Drop();
			}

			Item = item;
			Entity.Area.Remove(item);

			item.RemoveDroppedComponents();
			item.AddComponent(new OwnerComponent(Entity));

			var e = new ItemAddedEvent {
				Item = item
			};
			
			Send(e);
			item.HandleEvent(e);
		}
		
		public void Drop() {
			var e = new ItemRemovedEvent {
				Item = Item
			};
			
			Send(e);
			Item.HandleEvent(e);

			Item.Center = Entity.Center;
			Entity.Area.Add(Item);
			Item.RemoveComponent<OwnerComponent>();
			Item.AddDroppedComponents();
			Item = null;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Item != null && Item.Done) {
				Item = null;
			}
		}

		protected virtual bool ShouldReplace(Item item) {
			return Item == null;
		}

		public override bool HandleEvent(Event e) {
			if (e is ItemCheckEvent ev && ShouldReplace(ev.Item)) {
				Set(ev.Item);
				return true;
			}

			return base.HandleEvent(e);
		}

		public override void Save(FileWriter stream) {
			base.Save(stream); // todo
		}

		public override void Load(FileReader reader) {
			base.Load(reader); // todo
		}
	}
}