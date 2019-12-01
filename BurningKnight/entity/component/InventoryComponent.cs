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
		public bool Busy;

		public void Pickup(Item item, bool animate = true) {
			if (item == null) {
				return;
			}
		
			item.Unknown = false;
			Entity.Area.Remove(item);
			item.Done = false;
			
			if (!Send(new ItemCheckEvent {
				Item = item,
				Animate = animate
			})) {
				if (Entity is Player p && (item.Type == ItemType.Artifact || item.Type == ItemType.ConsumableArtifact)) {
					if (item.Type == ItemType.ConsumableArtifact) {
						p.AnimateItemPickup(item, () => {
							item.Use(p);
							item.Done = true;
						}, false);
					} else if (animate) {
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

			item.RemoveDroppedComponents();
			item.AddComponent(new OwnerComponent(Entity));

			item.Use(Entity);

			var e = new ItemAddedEvent {
				Item = item,
				Who = Entity
			};
			
			Send(e);		
			item.Touched = true;
		}

		public void RemoveAndDispose(string id) {
			foreach (var i in Items) {
				if (i.Id == id) {
					Remove(i, true);
					break;
				}
			}
		}
		
		public void Remove(Item item, bool dispose = false) {
			Items.Remove(item);

			var e = new ItemRemovedEvent {
				Item = item
			};
			
			Send(e);
			item.HandleEvent(e);

			if (dispose) {
				item.Done = true;
				return;
			}
			
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
				Entity.Area.Remove(item);

				item.Done = false;
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