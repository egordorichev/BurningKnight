using System.Collections.Generic;
using BurningKnight.assets.achievements;
using BurningKnight.assets.particle;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.level;
using BurningKnight.state;
using ImGuiNET;
using Lens.entity;
using Lens.entity.component;
using Lens.util.file;
using Lens.util.math;

namespace BurningKnight.entity.component {
	public class InventoryComponent : SaveableComponent {
		public List<Item> Items = new List<Item>();
		public bool Busy;

		public bool Pickup(Item item, bool animate = true) {
			if (item == null) {
				return false;
			}
			
			var e = new ItemCheckEvent {
				Item = item,
				Animate = animate
			};
			
			if (!Send(e)) {
				if (e.Blocked) {
					return false;
				}
				
				if (Entity is Player p && (item.Type == ItemType.Scourge || item.Type == ItemType.Artifact || item.Type == ItemType.ConsumableArtifact)) {
					if (item.Type == ItemType.ConsumableArtifact) {
						p.AnimateItemPickup(item, () => {
							item.Use(p);
							item.Done = true;

							if (item.Type == ItemType.Scourge) {
								Achievements.Unlock("bk:scourged");
							}
						}, false);
					} else if (animate) {
						p.AnimateItemPickup(item, () => {
							if (item.Type == ItemType.Scourge) {
								var center = Entity.Center;
			
								for (var i = 0; i < 10; i++) {
									var part = new ParticleEntity(Particles.Scourge());
						
									part.Position = center + Rnd.Vector(-4, 4);
									part.Particle.Scale = Rnd.Float(0.4f, 0.8f);
									Entity.Area.Add(part);
									part.Depth = 1;
								}
							}	
						});
					} else {
						Add(item);
					}
				}
			}
			
			item.Unknown = false;
			Entity.Area.Remove(item);
			item.Done = false;

			return true;
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

			if (Entity is Player && !item.Touched && item.Scourged) {
				Run.AddScourge(true);
			}
			
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
				Item = item,
				Owner = Entity
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