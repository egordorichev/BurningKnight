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
			return Item != null && Item.Id == id;
		}
		
		public virtual void Set(Item item) {
			var old = Item;
			
			if (Item != null) {
				Drop();
				Item = null;
			}

			item.RemoveDroppedComponents();
			item.AddComponent(new OwnerComponent(Entity));
			
			Entity.GetComponent<StateComponent>().PushState(new Player.GotState {
				Item = item,
				
				OnEnd = (i) => {
					Item = i;
					Entity.Area.Remove(i);

					var e = new ItemAddedEvent {
						Item = i,
						Old = old,
						Component = this
					};

					debugItem = i.Id;
			
					Send(e);
					i.HandleEvent(e);
				}
			});
		}
		
		public Item Drop() {
			var e = new ItemRemovedEvent {
				Item = Item
			};
			
			Send(e);
			Item.HandleEvent(e);

			Item.Center = Entity.Center;
			Entity.Area.Add(Item);
			Item.RemoveComponent<OwnerComponent>();
			Item.AddDroppedComponents();

			var item = Item;
			Item = null;

			return item;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Item != null) {
				if (Item.Done) {
					Item = null;
				} else {
					Item.Update(dt);
				}
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

				Set(item);
			}
		}

		private string debugItem = "";

		public override void RenderDebug() {
			if (ImGui.InputText("Item", ref debugItem, 128, ImGuiInputTextFlags.EnterReturnsTrue)) {
				Set(Items.CreateAndAdd(debugItem, Entity.Area));
			}

			if (Item != null) {
				AreaDebug.RenderEntity(Item);
			}
		}
	}
}