using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.level.entities;
using BurningKnight.state;
using Lens;
using Lens.entity;

namespace BurningKnight.entity.creature.player {
	public class WeaponComponent : ItemComponent {
		protected bool AtBack = true;

		public void Render(bool shadow) {
			if (Item != null && Item.Renderer != null) {
				if (!shadow) {
					var sh = Shaders.Item;
					Shaders.Begin(sh);
					sh.Parameters["time"].SetValue(Engine.Time * 0.1f);
					sh.Parameters["size"].SetValue(ItemGraphicsComponent.FlashSize);
				}

				Item.Renderer.Render(AtBack, Engine.Instance.State.Paused, Engine.Delta, shadow);

				if (!shadow) {
					Shaders.End();
				}
			}
		}
		
		protected override bool ShouldReplace(Item item) {
			return (Run.Depth > 0 || !AtBack) && item.Type == ItemType.Weapon;
		}

		public override bool HandleEvent(Event e) {
			if (e is ItemAddedEvent ev) {
				if (AtBack && ev.Old == null && Item != null && ev.Component == this) {
					Swap();
				}
			}

			return base.HandleEvent(e);
		}

		protected void Swap() {
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