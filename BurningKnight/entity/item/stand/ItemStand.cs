using System;
using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.state;
using ImGuiNET;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.item.stand {
	public class ItemStand : Prop {
		private static TextureRegion itemShadow;
		private static TextureRegion standShadow;
		private static Vector2 shadowOffset = new Vector2(3, 3);

		private Item item;
		
		public Item Item => item;

		public override void Init() {
			base.Init();

			if (itemShadow == null) {
				itemShadow = CommonAse.Props.GetSlice("item_shadow");
				standShadow = CommonAse.Props.GetSlice("stand_shadow");
			}
		}

		protected virtual void OnTake(Item item, Entity who) {
			
		}

		public virtual void SetItem(Item i, Entity entity, bool remove = true) {
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

				OnTake(item, entity);
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
			Graphics.Render(standShadow, Position + new Vector2(0, Height));
		}

		protected virtual bool CanTake(Entity entity) {
			return true;
		}

		private bool Interact(Entity entity) {
			if (Item != null && Item.Masked && Run.Depth < 1) {
				return false;
			}
			
			if (entity.TryGetComponent<InventoryComponent>(out var inventory)) {
				if (item != null) {
					if (CanTake(entity)) {
						var i = item;

						if (this is HatStand) {
							var ht = entity.GetComponent<HatComponent>();
							var it = ht.Item;

							ht.Set(null, false);
							SetItem(it, entity, false);
						} else if (this is PermanentStand) {
							var ht = entity.GetComponent<ActiveWeaponComponent>();
							var it = ht.Item;

							ht.Set(null, false);
							SetItem(it, entity, false);
						} else {
							SetItem(null, entity, false);
						}

						inventory.Pickup(i);
					}

					return false;
				} else if (entity.TryGetComponent<ActiveWeaponComponent>(out var weapon) && weapon.Item != null) {
					SetItem(weapon.Drop(), entity);
					return false;
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

		protected virtual bool CanInteract(Entity e) {
			return Item != null || (e.TryGetComponent<ActiveWeaponComponent>(out var w) && w.Item != null);
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

		protected bool dontSaveItem;

		public override void Load(FileReader stream) {
			base.Load(stream);

			if (dontSaveItem) {
				return;
			}
			
			if (stream.ReadBoolean()) {
				SetItem(Items.CreateAndAdd(stream.ReadString(), Area), null);
			}
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			if (dontSaveItem) {
				return;
			}
			
			stream.WriteBoolean(item != null);

			if (item != null) {
				stream.WriteString(item.Id);
			}
		}

		protected string debugItem = "";

		public override void RenderImDebug() {
			if (ImGui.InputText("Item", ref debugItem, 128, ImGuiInputTextFlags.EnterReturnsTrue)) {
				SetItem(Items.CreateAndAdd(debugItem, Area), null);
			}
		}
	}
}