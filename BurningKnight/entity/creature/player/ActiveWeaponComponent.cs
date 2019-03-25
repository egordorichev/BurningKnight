using Lens;
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

			if (Input.WasPressed(Controls.Swap)) {
				var component = Entity.GetComponent<WeaponComponent>();

				// Swap the items
				var tmp = component.Item;
				component.Item = Item;
				Item = tmp;
			}
		}
	}
}