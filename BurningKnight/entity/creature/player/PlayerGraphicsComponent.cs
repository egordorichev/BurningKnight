using BurningKnight.entity.component;
using Lens.input;
using Lens.util.camera;

namespace BurningKnight.entity.creature.player {
	public class PlayerGraphicsComponent : AnimationComponent {
		public PlayerGraphicsComponent() : base("gobbo") {}
		
		public override void Update(float dt) {
			base.Update(dt);
			Flipped = Entity.CenterX > Camera.Instance.ScreenToCamera(Input.Mouse.ScreenPosition).X;
		}

		public override void Render() {
			var weapon = GetComponent<WeaponComponent>();
			var activeWeapon = GetComponent<ActiveWeaponComponent>();
					
			weapon.Render();
			base.Render();
			activeWeapon.Render();
		}
	}
}