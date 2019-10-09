using BurningKnight.assets;
using BurningKnight.assets.input;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.level.entities;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens;
using Lens.entity;
using Lens.util;

namespace BurningKnight.entity.creature.player {
	public class WeaponComponent : ItemComponent {
		protected bool AtBack = true;
		private bool requestSwap;
		
		public override void PostInit() {
			base.PostInit();

			if (Item != null && Run.Depth < 1) {
				Item.Done = true;
				Item = null;
			}
		}

		public void Render(bool shadow, int offset) {
			if (Item != null && Item.Renderer != null) {
				if (!shadow) {
					var sh = Shaders.Item;
					Shaders.Begin(sh);
					sh.Parameters["time"].SetValue(Engine.Time * 0.1f);
					sh.Parameters["size"].SetValue(ItemGraphicsComponent.FlashSize);
				}

				Item.Renderer.Render(AtBack, Engine.Instance.State.Paused, Engine.Delta, shadow, offset);

				if (!shadow) {
					Shaders.End();
				}
			}
		}
		
		protected override bool ShouldReplace(Item item) {
			return (Run.Depth > 0 || !AtBack) && item.Type == ItemType.Weapon;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (requestSwap) {
				Swap();
				requestSwap = false;
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is ItemAddedEvent ev) {
				if (ev.Component == this) {
					ev.Old?.Drop();
					ev.Item?.Pickup();
					
					if (ev.Item != null && ev.Old == null && AtBack) {
						if (GlobalSave.IsTrue("control_swap")) {
							Entity.GetComponent<ActiveWeaponComponent>().requestSwap = true;
						} else {
							Entity.GetComponent<DialogComponent>().Dialog.Str.SetVariable("ctrl",
									Controls.Find(Controls.Swap, GamepadComponent.Current != null));

							Entity.GetComponent<DialogComponent>().Start("control_5");
						}
					}
				}
			}

			return base.HandleEvent(e);
		}

		protected void Swap() {
			if (GlobalSave.IsFalse("control_swap")) {
				GlobalSave.Put("control_swap", true);
				Entity.GetComponent<DialogComponent>().Close();
			}
			
			var component = AtBack ? Entity.GetComponent<ActiveWeaponComponent>() : Entity.GetComponent<WeaponComponent>();
			
			if (!Send(new WeaponSwappedEvent {
				Who = (Player) Entity,
				Old = Item,
				Current = component.Item
			})) {
				// Swap the items
				var tmp = component.Item;
				component.Item = Item;
				Item = tmp;

				if (!AtBack) {
					component.Item?.PutAway();
					Item?.TakeOut();
				} else {
					Log.Error("Swap is called from not active weapon component");
				}
			}
		}
	}
}