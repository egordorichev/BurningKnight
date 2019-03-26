using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.util;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.graphics;
using Microsoft.Xna.Framework;
using MonoGame.Extended.Sprites;
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
			
			// todo: big sensor body component
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
					inventory.Pickup(item);
					item = null;
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
			var component = GetComponent<InteractableComponent>();
			var renderOutline = component.OutlineAlpha > 0.05f;
			
			if (renderOutline) {
				var shader = Shaders.Entity;
				Shaders.Begin(shader);

				shader.Parameters["flash"].SetValue(component.OutlineAlpha);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(ColorUtils.White);

				foreach (var d in MathUtils.Directions) {
					Graphics.Render(((SliceComponent) GraphicsComponent).Sprite, Position + d);
				}
				
				Shaders.End();
			}

			GraphicsComponent.Render();
			
			if (item == null) {
				return;
			}

			var region = item.Region;
			var pos = new Vector2(CenterX, CenterY - region.Source.Height / 2f);

			if (renderOutline) {
				var shader = Shaders.Entity;
				Shaders.Begin(shader);

				shader.Parameters["flash"].SetValue(component.OutlineAlpha);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(ColorUtils.White);

				foreach (var d in MathUtils.Directions) {
					Graphics.Render(region, pos + d, 0, region.Center);
				}
				
				Shaders.End();
			}
			
			Graphics.Render(region, pos, 0, region.Center);
		}
	}
}