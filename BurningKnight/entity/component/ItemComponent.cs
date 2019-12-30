using BurningKnight.assets.items;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.state;
using BurningKnight.util;
using ImGuiNET;
using Lens.entity;
using Lens.entity.component;
using Lens.entity.component.logic;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class ItemComponent : SaveableComponent {
		public Item Item { get; protected set; }
		public bool DontSave;

		public bool Has(string id) {
			return Item?.Id == id;
		}
		
		public virtual void Set(Item item, bool animate = true) {
			var prev = Item;
			
			if (!animate && Item != null) {
				Drop();
				Item = null;

	#if DEBUG
				debugItem = "";
	#endif
			}

			if (item == null) {
				Item = null;
				return;
			}
			
			Entity.Area.Remove(item);
			item.RemoveDroppedComponents();
			item.AddComponent(new OwnerComponent(Entity));

			if (!animate) {
				SetupItem(item, prev);
				return;
			}
			
			if (Item != null) {
				Drop();
				Item = null;
#if DEBUG
				debugItem = "";
#endif
			}
			
			if (Entity is Player p) {
				p.AnimateItemPickup(item, () => {
					SetupItem(item, prev);
				}, false);
			} else {
				SetupItem(item, prev);
			}
		}

		private void SetupItem(Item item, Item previous) {
			Send(new ItemAddedEvent {
				Item = item,
				Component = this,
				Who = Entity,
				Old = previous
			});
			
			Item = item;

			if (Entity is Player && !item.Touched && item.Scourged) {
				Run.AddScourge(true);
			}

			item.Done = false;
			item.Touched = true;
			item.RemoveDroppedComponents();
		
	#if DEBUG
			debugItem = item.Id;
	#endif

			OnItemSet(previous);
		}

		protected virtual void OnItemSet(Item previous) {
			
		}
		
		public Item Drop() {
			var e = new ItemRemovedEvent {
				Item = Item
			};
			
			Send(e);

			Item.Center = Entity.Center - new Vector2(0, 4);
			Entity.Area.Add(Item);
			Item.RemoveComponent<OwnerComponent>();
			Item.AddDroppedComponents();

			var item = Item;
			Item = null;

	#if DEBUG
			debugItem = "";
	#endif

			return item;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Item != null) {
				if (Item.Done) {
					Item = null;

	#if DEBUG
					debugItem = "";
	#endif
				} else {
					Item.Center = Entity.Center;
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
			
			if (e is ItemCheckEvent ev && !ev.Handled && ShouldReplace(ev.Item)) {
				if (Entity is Player && Item != null && Item.Scourged) {
					AnimationUtil.ActionFailed();
					ev.Blocked = true;
					
					return false;
				}
				
				Set(ev.Item, ev.Animate);
				return true;
			}

			return base.HandleEvent(e);
		}

		public override void Save(FileWriter stream) {
			if (DontSave) {
				stream.WriteBoolean(false);
				return;
			}
 			
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

		public void Exchange(ItemComponent component) {
			var tmp = component.Item;
			component.Item = Item;

			if (Item != null) {
				if (!Item.HasComponent<OwnerComponent>()) {
					Item.AddComponent(new OwnerComponent(component.Entity));
				} else {
					Item.GetComponent<OwnerComponent>().Owner = component.Entity;
				}
			}

			Item = tmp;

			if (Item == null) {
				return;
			}
			
			if (!Item.HasComponent<OwnerComponent>()) {
				Item.AddComponent(new OwnerComponent(Entity));
			} else {
				Item.GetComponent<OwnerComponent>().Owner = Entity;
			}
		}
		
		#if DEBUG
		private string debugItem = "";

		public override void RenderDebug() {
			if (Item != null) {
				ImGui.Text($"Use time: {Item.UseTime}");
				ImGui.Text($"Delay: {Item.Delay}");
			}
			
			if (ImGui.InputText("Item", ref debugItem, 128, ImGuiInputTextFlags.EnterReturnsTrue)) {
				var item = Items.Create(debugItem);
				Set(item);
			}
		}
		#endif

		public void Cleanse() {
			if (Item != null && Item.Scourged) {
				Item.Scourged = false;
			}
		}
	}
}