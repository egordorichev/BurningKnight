using System.Collections.Generic;
using System.IO;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using ImGuiNET;
using Lens.entity.component;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.entity.component {
	public class InventoryComponent : SaveableComponent {
		public List<Item> Items = new List<Item>();

		public void Pickup(Item item) {
			if (!Send(new ItemCheckEvent {
				Item = item
			})) {
				if (Entity is Player && (item.Type == ItemType.Artifact || item.Type == ItemType.Active || 
				                         item.Type == ItemType.Lamp || item.Type == ItemType.Weapon)) {
					
					Entity.GetComponent<StateComponent>().PushState(new Player.GotState {
						Item = item
					});
				} else {
					item.Use(Entity);
					Add(item);	
				}
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
				
				Pickup(item);
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