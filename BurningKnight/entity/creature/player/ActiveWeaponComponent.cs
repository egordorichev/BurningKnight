using Lens.input;

namespace BurningKnight.entity.creature.player {
	public class ActiveWeaponComponent : WeaponComponent {
		public override void Update(float dt) {
			base.Update(dt);

			if (Item != null && Input.WasPressed(Controls.Use)) {
				
			}
		}
	}
}