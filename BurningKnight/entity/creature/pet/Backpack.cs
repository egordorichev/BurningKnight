using System;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.entity.item;
using BurningKnight.save;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.util;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.pet {
	public class Backpack : Pet {
		private InteractFx fx;
		private bool open;
		private TextureRegion itemRegion;
		
		public override void AddComponents() {
			base.AddComponents();

			Width = 12;
			
			AddComponent(new FollowerComponent {
				MaxDistance = 96,
				FollowSpeed = 2
			});
			
			AddComponent(new AnimationComponent("backpack") {
				ShadowOffset = -2
			});
			
			AddComponent(new ShadowComponent(RenderShadow));
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Dynamic, true));
			AddComponent(new ItemComponent());
			
			AddComponent(new InteractableComponent(Interact) {
				CanInteract = e => e == Owner && (e.GetComponent<ActiveWeaponComponent>().Item != null || GetComponent<ItemComponent>().Item != null),
				OnStart = e => AddFx()
			});

			try {
				var id = GameSave.GetString("backpack");

				if (id != null) {
					var item = Items.CreateAndAdd(id, Area);
					GetComponent<ItemComponent>().Set(item, false);
					item.GetComponent<OwnerComponent>().Owner = Owner;
					itemRegion = item.Region;
				}
			} catch (Exception e) {
				Log.Error(e);
			}
			
			GetComponent<AnimationComponent>().Animate();
		}

		public override void Destroy() {
			base.Destroy();
			GameSave.Put("backpack", GetComponent<ItemComponent>().Item?.Id);
		}

		private void AddFx() {
			if (fx != null && !fx.Done) {
				fx.Close();
			}
			
			var i = GetComponent<ItemComponent>().Item;
			Engine.Instance.State.Ui.Add(fx = new InteractFx(this, i == null ? Locale.Get("place_an_item") : i.Name));
		}

		private bool Interact(Entity entity) {
			var w = entity.GetComponent<ActiveWeaponComponent>();
			var i = GetComponent<ItemComponent>();
			var w2 = entity.GetComponent<WeaponComponent>();

			if (i.Item != null && w2.Item == null) {
				i.Exchange(w2);
				w.RequestSwap();
			} else {
				i.Exchange(w);
			}

			if (w.Item != null) {
				Audio.PlaySfx(w.Item.Data.WeaponType.GetSwapSfx());
				entity.GetComponent<PlayerGraphicsComponent>().AnimateSwap();
			} else if (entity.GetComponent<WeaponComponent>().Item != null) {
				w.RequestSwap();
			}


			if (i.Item != null) {
				itemRegion = i.Item.Region;
			} else {
				itemRegion = null;
			}

			AddFx();
			return false;
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				if (!open && cse.Entity == Owner) {
					GetComponent<AnimationComponent>().Animate(() => {
						GetComponent<AnimationComponent>().Animation.Tag = "open";
					});

					GetComponent<FollowerComponent>().Paused = true;
					GetComponent<RectBodyComponent>().Velocity *= 0.5f;

					Audio.PlaySfx("unlock");
					open = true;
				}
			} else if (e is CollisionEndedEvent cee) {
				if (open && cee.Entity == Owner) {
					GetComponent<AnimationComponent>().Animate(() => {
						open = false;
						GetComponent<AnimationComponent>().Animation.Tag = "idle";
					});

					GetComponent<FollowerComponent>().Paused = false;
					
					Audio.PlaySfx("swap");
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Render() {
			base.Render();

			if (open && itemRegion != null) {
				Graphics.Render(itemRegion, Position + new Vector2(6, 7), 0, itemRegion.Center, GetComponent<AnimationComponent>().Scale);
			}
		}
	}
}