using System;
using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.level;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.editor;
using BurningKnight.util;
using ImGuiNET;
using Lens;
using Lens.entity;
using Lens.entity.component.graphics;
using Lens.graphics;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.item {
	public class ItemStand : SaveableEntity, PlaceableEntity {
		private static TextureRegion itemShadow;
		private static Vector2 shadowOffset = new Vector2(3, 3);
		
		private Item item;
		
		public Item Item => item;

		public override void Init() {
			base.Init();

			if (itemShadow == null) {
				itemShadow = CommonAse.Props.GetSlice("item_shadow");
			}
		}

		public void SetItem(Item i, Entity entity, bool remove = true) {
			if (item == i) {
				return;
			}

			if (item != null) {
				if (remove) {
					item.AddDroppedComponents();
					item.RemoveComponent<OwnerComponent>();
				}

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
				item.CenterX = CenterX;
				item.Bottom = Y + 6;
				item.AutoPickup = false;
				
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

		public override void Update(float dt) {
			base.Update(dt);
			
			if (item != null) {
				item.Center = Center;
			}
		}

		public override void AddComponents() {
			base.AddComponents();

			var g = new SliceComponent("props", GetSprite());
			AddComponent(g);
			g.SetOwnerSize();
			
			AddTag(Tags.Item);
			
			AddComponent(new RectBodyComponent(2, 2, Width - 4, Height - 9, BodyType.Static));
			AddComponent(new SensorBodyComponent(0, 0, Width, Height, BodyType.Static));

			AddComponent(new InteractableComponent(Interact) {
				CanInteract = CanInteract,
				OnStart = OnInteractionStart
			});
			
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new RoomComponent());
		}

		protected virtual string GetSprite() {
			return "slab_a";
		}

		private void RenderShadow() {
			GraphicsComponent.Render(true);
		}

		protected virtual bool CanTake(Entity entity) {
			return true;
		}

		private bool Interact(Entity entity) {
			if (Item != null && Item.Masked) {
				return false;
			}
			
			if (entity.TryGetComponent<InventoryComponent>(out var inventory)) {
				if (item != null) {
					if (CanTake(entity)) {
						var i = item;

						SetItem(null, entity, false);
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
					Engine.Instance.State.Ui.Add(new ItemPickupFx(item));
				}
			}
		}

		private bool CanInteract(Entity e) {
			return true;
		}

		public override void Render() {
			if (!TryGetComponent<InteractableComponent>(out var component)) {
				return;
			}
			
			var renderOutline = component.OutlineAlpha > 0.05f;
			
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

			Graphics.Color = Level.ShadowColor;
			Graphics.Render(itemShadow, Position + shadowOffset);
			Graphics.Color = ColorUtils.WhiteColor;

			var t = item.Animation == null ? item.GetComponent<ItemGraphicsComponent>().T : 0;
			var angle = (float) Math.Cos(t * 3f) * 0.4f;
			
			var region = item.Region;
			var animated = item.Animation != null;
			var pos = item.Center + new Vector2(0, (animated ? 0 : (float) (Math.Sin(t * 3f) * 0.5f + 0.5f) * -5.5f) - 5.5f);
			
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
				if (item.Masked) {
					Graphics.Color = ItemGraphicsComponent.MaskedColor;
					Graphics.Render(region, pos, angle, region.Center);
					Graphics.Color = ColorUtils.WhiteColor;
				} else {
					var shader = Shaders.Item;
				
					Shaders.Begin(shader);
					shader.Parameters["time"].SetValue(t * 0.1f);
					shader.Parameters["size"].SetValue(ItemGraphicsComponent.FlashSize);

					Graphics.Render(region, pos, angle, region.Center);
					Shaders.End();
				}
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);

			if (stream.ReadBoolean()) {
				SetItem(Items.CreateAndAdd(stream.ReadString(), Area), null);
			}
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(item != null);

			if (item != null) {
				stream.WriteString(item.Id);
			}
		}
		
		private string debugItem = "";

		public override void RenderImDebug() {
			if (ImGui.InputText("Item", ref debugItem, 128, ImGuiInputTextFlags.EnterReturnsTrue)) {
				SetItem(Items.CreateAndAdd(debugItem, Area), null);
			}
		}
	}
}