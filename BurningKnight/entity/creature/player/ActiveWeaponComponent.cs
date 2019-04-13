using BurningKnight.entity.events;
using BurningKnight.entity.item;
using Lens.input;

namespace BurningKnight.entity.creature.player {
	public class ActiveWeaponComponent : WeaponComponent {
		private bool stopped = true;
		
		public ActiveWeaponComponent() {
			AtBack = false;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Item != null && Input.WasPressed(Controls.Use)) {
				Item.Use((Player) Entity);
				Item.Renderer?.OnUse();
			}
	
			if (Input.WasPressed(Controls.Swap) || (Input.Mouse.WheelDelta != 0 && stopped)) {
				stopped = false;
				Swap();
			}

			stopped = Input.Mouse.WheelDelta == 0;
		}

		protected override bool ShouldReplace(Item item) {
			return base.ShouldReplace(item) && (Item == null || Entity.GetComponent<WeaponComponent>().Item != null);
		}
	}
}