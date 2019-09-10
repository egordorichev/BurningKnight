using System;
using BurningKnight.assets.input;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using BurningKnight.state;
using Lens.input;

namespace BurningKnight.entity.creature.player {
	public class ActiveWeaponComponent : WeaponComponent {
		private bool stopped = true;
		
		public ActiveWeaponComponent() {
			AtBack = false;
		}

		public override void Update(float dt) {
			base.Update(dt);

			var controller = GetComponent<GamepadComponent>().Controller;

			if (Item != null) {
				if (GetComponent<BuffsComponent>().Has<CharmedBuff>()) {
					return;
				}
				
				if (Input.WasPressed(Controls.Use, controller) || (Item.Automatic && Input.IsDown(Controls.Use, controller) && Item.Delay <= 0.001f)) {
					Item.Use((Player) Entity);
				}
			}
	
			if ((Input.WasPressed(Controls.Swap, controller) || (Input.Mouse.WheelDelta != 0 && stopped)) && Run.Depth > 0) {
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