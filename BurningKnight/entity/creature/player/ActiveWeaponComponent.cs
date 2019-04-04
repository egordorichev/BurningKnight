using BurningKnight.entity.events;
using Lens.input;

namespace BurningKnight.entity.creature.player {
	public class ActiveWeaponComponent : WeaponComponent {
		public ActiveWeaponComponent() {
			AtBack = false;
		}
		
		public override void Update(float dt) {
			base.Update(dt);

			if (Item != null && Input.WasPressed(Controls.Use)) {
				Item.Use((Player) Entity);
				Item.Renderer?.OnUse();
			}
	
			var component = Entity.GetComponent<WeaponComponent>();

			if (Input.WasPressed(Controls.Swap) && !Send(new WeaponSwappedEvent {
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