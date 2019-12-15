using System;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.save;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.util;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.creature.pet {
	public class Backpack : Pet {
		private InteractFx fx;
		
		public override void AddComponents() {
			base.AddComponents();

			Width = 12;
			
			AddComponent(new FollowerComponent {
				MaxDistance = 32
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
				}
			} catch (Exception e) {
				Log.Error(e);
			}
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
			GetComponent<ItemComponent>().Exchange(w);

			if (w.Item == null && entity.GetComponent<WeaponComponent>().Item != null) {
				w.RequestSwap();
			}
			
			AddFx();
			return false;
		}

		public override bool HandleEvent(Event e) {
			if (e is CollisionStartedEvent cse) {
				if (cse.Entity == Owner) {
					GetComponent<AnimationComponent>().Animation.Tag = "open";
					GetComponent<FollowerComponent>().Paused = true;
					GetComponent<RectBodyComponent>().Velocity *= 0.5f;
				}
			} else if (e is CollisionEndedEvent cee) {
				if (cee.Entity == Owner) {
					GetComponent<AnimationComponent>().Animation.Tag = "idle";
					GetComponent<FollowerComponent>().Paused = false;
				}
			}
			
			return base.HandleEvent(e);
		}
	}
}