using BurningKnight.entity.component;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.graphics;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.item {
	public class ItemStand : Entity {
		private Item item;

		public Item Item {
			get => item;
			
			set {
				if (item == value) {
					return;
				}
				
				item?.AddDroppedComponents();
				item?.RemoveComponent<OwnerComponent>();
				
				item = value;
				
				item?.RemoveDroppedComponents();
				item?.AddComponent(new OwnerComponent(this));
			}
		}

		public ItemStand() {
			Width = 14;
			Height = 14;
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new RectBodyComponent(2, 2, 10, 1, BodyType.Static));
			AddComponent(new InteractableComponent(Interact) {
				CanInteract = CanInteract
			});
			
			SetGraphicsComponent(new SliceComponent("props", "slab_a"));
		}

		private bool Interact(Entity entity) {
			if (entity.TryGetComponent<InventoryComponent>(out var inventory)) {
				if (item != null) {
					item.RemoveComponent<OwnerComponent>();
					item = null;
					inventory.Pickup(item);
				} else {
					// todo: take active weapon
				}
			}

			return true;
		}

		private bool CanInteract() {
			return true; // item != null;
		}
		
		public override void Render() {
			base.Render();

			if (item == null) {
				return;
			}

			var region = item.Region;
			Graphics.Render(region, new Vector2(CenterX, CenterY - region.Source.Height / 2f), 0, region.Center);
		}
	}
}