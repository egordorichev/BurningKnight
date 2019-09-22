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
				if (AtBack && ev.Old == null && ev.Item != null && ev.Component == this) {
					if (GlobalSave.IsTrue("control_swap")) {
						requestSwap = true;
					} else {
						Entity.GetComponent<DialogComponent>().Start("control_5");
						GetComponent<DialogComponent>().Dialog.Str.SetVariable("ctrl", Controls.Find(Controls.Swap, GamepadComponent.Current != null));
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
			}
		}
	}
}