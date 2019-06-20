using BurningKnight.assets.items;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.state;
using ImGuiNET;
using Lens.entity;
using Lens.entity.component;
using Lens.entity.component.logic;
using Lens.util.file;

namespace BurningKnight.entity.component {
	public class ItemComponent : SaveableComponent {
		public Item Item { get; protected set; }

		public bool Has(string id) {
			return Item?.Id == id;
		}
		
		public virtual void Set(Item item, bool animate = true) {
			if (!animate && Item != null) {
				Drop();
				Item = null;
				debugItem = "";
			}

			if (item == null) {
				Item = null;
				return;
			}
			
			Entity.Area.Remove(item);
			item.RemoveDroppedComponents();
			item.AddComponent(new OwnerComponent(Entity));

			if (!animate) {
				SetupItem(item);
				return;
			}
			
			((Player) Entity).AnimateItemPickup(item, () => {
				if (Item != null) {
					Drop();
					Item = null;
					debugItem = "";
				}

				SetupItem(item);
			}, false);
		}

		private void SetupItem(Item item) {
			Send(new ItemAddedEvent {
				Item = item,
				Component = this,
				Who = Entity
			});
			
			Item = item;

			item.Done = false;
			item.Touched = true;
			item.RemoveDroppedComponents();

			debugItem = item.Id;
			OnItemSet();
		}

		protected virtual void OnItemSet() {
			
		}
		
		public Item Drop() {
			var e = new ItemRemovedEvent {
				Item = Item
			};
			
			Send(e);

			Item.Center = Entity.Center;
			Entity.Area.Add(Item);
			Item.RemoveComponent<OwnerComponent>();
			Item.AddDroppedComponents();

			var item = Item;
			Item = null;

			debugItem = "";

			return item;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Item != null) {
				if (Item.Done) {
					Item = null;
					debugItem = "";
				} else {
					Item.Update(dt);
				}
			}
		}

		protected virtual bool ShouldReplace(Item item) {
			return Item == null;
		}

		public override bool HandleEvent(Event e) {
			if (Item != null && Item.HandleOwnerEvent(e)) {
				return true;
			}
			
			if (e is ItemCheckEvent ev && ShouldReplace(ev.Item)) {
				Set(ev.Item, ev.Animate);
				return true;
			}

			return base.HandleEvent(e);
		}

		public override void Save(FileWriter stream) {
			stream.WriteBoolean(Item != null);
			Item?.Save(stream);
		}

		public override void Load(FileReader stream) {
			if (stream.ReadBoolean()) {
				var item = new Item();

				Entity.Area.Add(item, false);
				
				item.Load(stream);
				item.LoadedSelf = false;
				item.PostInit();

				Set(item, false);
			}
		}

		#if DEBUG
		private string debugItem = "";

		public override void RenderDebug() {
			if (ImGui.InputText("Item", ref debugItem, 128, ImGuiInputTextFlags.EnterReturnsTrue)) {
				var item = Items.Create(debugItem);
				Set(item);
			}
		}
		#endif
	}
}