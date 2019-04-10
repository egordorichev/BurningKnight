using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.util;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.graphics;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.item {
	public class ItemStand : Entity {
		private Item item;
		private float T;
		
		public Item Item => item;

		public void SetItem(Item i, Entity entity) {
			if (item == i) {
				return;
			}

			if (item != null) {
				item.AddDroppedComponents();
				item.RemoveComponent<OwnerComponent>();

				HandleEvent(new ItemTakenEvent {
					Item = item,
					Who = entity,
					Stand = this
				});
			}

			item = i;
				
			if (item != null) {
				item.RemoveDroppedComponents();
				item.AddComponent(new OwnerComponent(this));
				item.Center = Center;
				
				HandleEvent(new ItemPlacedEvent {
					Item = item,
					Who = entity,
					Stand = this
				});
			}
		}

		public ItemStand() {
			Width = 14;
			Height = 14;
		}

		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new RectBodyComponent(2, 2, 10, 5, BodyType.Static));
			AddComponent(new SensorBodyComponent(0, 0, Width, Height, BodyType.Static));

			AddComponent(new InteractableComponent(Interact) {
				CanInteract = CanInteract,
				OnStart = OnInteractionStart
			});
			
			AddComponent(new SliceComponent("props", "slab_a"));
		}

		protected virtual bool CanTake(Entity entity) {
			return true;
		}

		private bool Interact(Entity entity) {
			if (entity.TryGetComponent<InventoryComponent>(out var inventory)) {
				if (item != null) {
					if (CanTake(entity)) {
						var i = item;

						item.RemoveComponent<OwnerComponent>();
						SetItem(null, entity);
						inventory.Pickup(i);

						GetComponent<InteractableComponent>().OutlineAlpha = 0;
					}
				} else if (entity.TryGetComponent<ActiveWeaponComponent>(out var weapon) && weapon.Item != null) {
					SetItem(weapon.Drop(), entity);
					GetComponent<InteractableComponent>().OutlineAlpha = 0;
				}
			}

			return false;
		}
		
		private void OnInteractionStart(Entity entity) {
			if (item != null && entity is LocalPlayer) {
				if (item.AutoPickup) {
					item.OnInteractionStart(entity);
					item = null;
				} else {
					Area.Add(new ItemPickupFx(item));
				}
			}
		}

		public override void Update(float dt) {
			base.Update(dt);
			T += dt;
		}

		private bool CanInteract() {
			return true; // item != null;
		}

		public override void Render() {
			var component = GetComponent<InteractableComponent>();
			var renderOutline = component.OutlineAlpha > 0.05f;
			var t = T;
			var angle = (float) Math.Cos(t * 3f) * 0.4f;
			
			if (item == null && renderOutline) {
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

			GraphicsComponent.Render(false);
			
			if (item == null) {
				return;
			}

			var region = item.Region;
			var animated = item.Animation != null;
			var pos = item.Center + new Vector2(0, animated ? 0 : (float) (Math.Sin(t * 2f) * 0.5f + 0.5f) * -5.5f);
			
			if (renderOutline) {
				var shader = Shaders.Entity;
				Shaders.Begin(shader);

				shader.Parameters["flash"].SetValue(component.OutlineAlpha);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(ColorUtils.White);

				foreach (var d in MathUtils.Directions) {
					Graphics.Render(region, pos + d, animated ? 0 : angle, region.Center);
				}
				
				Shaders.End();
			}

			if (animated) {
				Graphics.Render(region, pos, 0, region.Center);				
			} else {
				var sh = Shaders.Item;
				Shaders.Begin(sh);
				sh.Parameters["time"].SetValue(t * 0.1f);
				sh.Parameters["size"].SetValue(ItemGraphicsComponent.FlashSize);
			
				Graphics.Render(region, pos, angle, region.Center);
			
				Shaders.End();	
			}
		}
	}
}