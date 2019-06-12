using System.Collections.Generic;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using ImGuiNET;
using Lens.entity;
using Lens.entity.component;
using Lens.util.file;

namespace BurningKnight.entity.component {
	public class InventoryComponent : SaveableComponent {
		public List<Item> Items = new List<Item>();

		public void Pickup(Item item, bool animate = true) {
			if (!Send(new ItemCheckEvent {
				Item = item,
				Animate = animate
			})) {
				if (Entity is Player p && item.Type == ItemType.Artifact) {
					if (animate) {
						p.AnimateItemPickup(item);
					} else {
						Add(item);
					}
				}
			}
		}

		public bool Has(string id) {
			foreach (var i in Items) {
				if (i.Id == id) {
					return true;
				}
			}

			return false;
		}
		
		public void Add(Item item) {
			if (item.HasComponent<OwnerComponent>()) {
				item.RemoveComponent<OwnerComponent>();
			}
			
			Items.Add(item);
			Entity.Area.Remove(item);
			item.Done = false;

			item.RemoveDroppedComponents();
			item.AddComponent(new OwnerComponent(Entity));

			item.Use(Entity);

			var e = new ItemAddedEvent {
				Item = item,
				Who = Entity
			};
			
			Send(e);
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

		public override bool HandleEvent(Event e) {
			foreach (var item in Items) {
				if (item.HandleOwnerEvent(e)) {
					return true;
				}
			}
			
			return base.HandleEvent(e);
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
			stream.WriteInt16((short) Items.Count);
			
			foreach (var item in Items) {
				item.Save(stream);
			}
		}

		public override void Load(FileReader stream) {
			var count = stream.ReadInt16();

			for (var i = 0; i < count; i++) {
				var item = new Item();

				Entity.Area.Add(item, false);
				
				item.Load(stream);
				item.LoadedSelf = false;
				item.PostInit();
				
				Pickup(item, false);
			}
		}

		public override void RenderDebug() {
			ImGui.Text($"Total {Items.Count} items");
			
			foreach (var item in Items) {
				ImGui.BulletText(item.Id);
			}
		}
	}
}