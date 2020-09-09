using System;
using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.physics;
using BurningKnight.state;
using BurningKnight.util;
using ImGuiNET;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Lens.util.file;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.item.stand {
	public class ItemStand : Prop, CollisionFilterEntity {
		private static TextureRegion itemShadow;
		private static TextureRegion standShadow;
		private static Vector2 shadowOffset = new Vector2(3, 3);

		protected Item item;
		
		public Item Item => item;
		public bool Hidden;

		public bool ShouldCollide(Entity entity) {
			return !Hidden;
		}

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

				if (item.HasComponent<OwnerComponent>()) {
					item.RemoveComponent<OwnerComponent>();
				}
				
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

				if (item.Done) {
					item = null;
				}
			}
		}

		public override void AddComponents() {
			base.AddComponents();

			var g = new SliceComponent("props", GetSprite());
			AddComponent(g);
			g.SetOwnerSize();
			
			AddTag(Tags.Item);
			
			var body = new RectBodyComponent(0, 4, 14, 10);
			AddComponent(body);
			body.Body.Mass = 100000000f;
			
			AddComponent(new SensorBodyComponent(-2, -2, Width + 4, Height + 4, BodyType.Static));

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
			if (!Hidden) {
				Graphics.Render(standShadow, Position + new Vector2(0, Height));
			}
		}

		protected virtual bool CanTake(Entity entity) {
			return true;
		}

		protected virtual bool Interact(Entity entity) {
			if (Item != null && Item.Masked && Run.Depth < 1) {
				return false;
			}
			
			if (entity.TryGetComponent<InventoryComponent>(out var inventory)) {
				if (item != null) {
					if (CanTake(entity)) {
						var i = item;
						var remove = false;

						if (this is GarderobeStand) {
							return true;
						} else if (this is PermanentStand && Item != null && Item.Type == ItemType.Weapon) {
							var c = entity.GetComponent<ActiveWeaponComponent>();
							var item = c.Item;

							c.Set(Items.CreateAndAdd(Item.Id, Area), false);

							if (item != null) {
								item.Done = true;
							}

							return true;
						} else if (this is PermanentStand && Item != null && Item.Type == ItemType.Active) {
							var c = entity.GetComponent<ActiveItemComponent>();
							var item = c.Item;

							c.Set(Items.CreateAndAdd(Item.Id, Area), false);

							if (item != null) {
								item.Done = true;
							}
							
							return true;
						} else if (this is LampStand && Item != null && Item.Type == ItemType.Lamp) {
							var c = entity.GetComponent<LampComponent>();
							var item = c.Item;

							c.Set(Items.CreateAndAdd(Item.Id, Area), false);

							if (item != null) {
								item.Done = true;
							}
							
							return true;
						} else {
							SetItem(null, entity, false);
							remove = true;
						}

						if (!inventory.Pickup(i) && remove) {
							SetItem(i, null, false);
						}
					}

					return this is HatStand || this is ShopStand || Run.Depth == -2;
				} else if (!(this is ShopStand) && entity.TryGetComponent<ActiveWeaponComponent>(out var weapon) && weapon.Item != null) {
					if (weapon.Item.Scourged) {
						AnimationUtil.ActionFailed();
					} else {
						SetItem(weapon.Drop(), entity);
						weapon.RequestSwap();
					}

					return true;
				}
			}

			return true;
		}
		
		private void OnInteractionStart(Entity entity) {
			if (item != null && entity is LocalPlayer) {
				if (item.AutoPickup) {
					item.OnInteractionStart(entity);
					item = null;
				} else if (Run.Depth != -2) {
					Engine.Instance.State.Ui.Add(new ItemPickupFx(item));
				}
			}
		}

		protected virtual bool CanInteract(Entity e) {
			return !Hidden && (Item != null || Run.Depth != -2);
		}

		public override void Render() {
			if (Hidden || !TryGetComponent<InteractableComponent>(out var component)) {
				return;
			}

			var cursed = item != null && item.Scourged;
			var interact = component.OutlineAlpha > 0.05f;
			var renderOutline = interact || cursed;
			
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
			var pos = item.Center + new Vector2(0, (animated ? 0 : (float) (Math.Sin(t * 3f) * 0.5f + 0.5f) * -5.5f - 3) - 5.5f);
			
			if (renderOutline) {
				var shader = Shaders.Entity;
				Shaders.Begin(shader);

				shader.Parameters["flash"].SetValue(cursed ? 1f : component.OutlineAlpha);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(!cursed ? ColorUtils.White : ColorUtils.Mix(ItemGraphicsComponent.ScourgedColor, ColorUtils.White, component.OutlineAlpha));

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
				var item = new Item();

				Area.Add(item, false);
				
				item.Load(stream);
				item.LoadedSelf = false;
				item.PostInit();

				SetItem(item, null);
			}
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			
			if (!dontSaveItem) {
				stream.WriteBoolean(item != null);
				item?.Save(stream);
			}
		}

		protected string debugItem = "";

		public override void RenderImDebug() {
			if (ImGui.InputText("Item", ref debugItem, 128, ImGuiInputTextFlags.EnterReturnsTrue)) {
				var item = Item;
				SetItem(Items.CreateAndAdd(debugItem, Area), null);

				if (item != null) {
					item.Done = true;
				}
			}
		}

		public virtual ItemPool GetPool() {
			return ItemPool.Treasure;
		}
	}
}