using System;
using BurningKnight.assets.input;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens.entity.component.logic;
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
				var b = GetComponent<BuffsComponent>();
				
				if (b.Has<FrozenBuff>() || b.Has<CharmedBuff>() || GetComponent<StateComponent>().StateInstance is Player.RollState) {
					return;
				}
				
				if (Input.WasPressed(Controls.Use, controller) || (Item.Automatic && Input.IsDown(Controls.Use, controller) && Item.Delay <= 0.001f)) {
					if (Run.Depth == -2) {
						GetComponent<DialogComponent>().Close();
					}
					
					Item.Use((Player) Entity);
				}
			}
	
			if ((Input.WasPressed(Controls.Swap, controller) || (Input.Mouse.WheelDelta != 0 && stopped)) && Run.Depth > 0 && GetComponent<WeaponComponent>().Item != null) {
				stopped = false;
				Swap();
				Entity.GetComponent<AudioEmitterComponent>().EmitRandomized("swap");
			}

			stopped = Input.Mouse.WheelDelta == 0;
		}

		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Weapon && (Item == null || Run.Depth < 1 || Entity.GetComponent<WeaponComponent>().Item != null);
		}

		protected override void OnItemSet(Item previous) {
			base.OnItemSet(previous);
			
			previous?.PutAway();
			Item?.TakeOut();

			if (Run.Depth == -2) {
				Entity.GetComponent<DialogComponent>().Dialog?.Str?.SetVariable("ctrl", Controls.Find(Controls.Use, GamepadComponent.Current != null));
				Entity.GetComponent<DialogComponent>().Start("control_2");
			}
		}
	}
}