using System;
using BurningKnight.assets;
using BurningKnight.assets.input;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens.assets;
using Lens.entity.component.logic;
using Lens.input;

namespace BurningKnight.entity.creature.player {
	public class ActiveWeaponComponent : WeaponComponent {
		private bool stopped = true;
		private float timeSinceReady;
		
		public ActiveWeaponComponent() {
			AtBack = false;
		}

		public override void Update(float dt) {
			base.Update(dt);

			var controller = GetComponent<GamepadComponent>().Controller;

			if (Item != null) {
				var ready = Item.Delay <= 0.001f;

				if (ready) {
					timeSinceReady += dt;
				} else {
					timeSinceReady = 0;
				}
				
				var b = GetComponent<BuffsComponent>();
				
				if (b.Has<FrozenBuff>() || b.Has<CharmedBuff>() || GetComponent<StateComponent>().StateInstance is Player.RollState) {
					return;
				}
				
				if ((Input.WasPressed(Controls.Use, controller) || (controller != null && (
						controller.DPadDownCheck || controller.DPadLeftCheck || controller.DPadUpCheck || controller.DPadRightCheck                                                  
				  ))) || ((Item.Automatic || timeSinceReady > 0.2f) && Input.IsDown(Controls.Use, controller) && ready)) {

					if (GetComponent<StateComponent>().StateInstance is Player.SleepingState) {
						GetComponent<StateComponent>().Become<Player.IdleState>();
					}
					
					if (Run.Depth == -2) {
						GetComponent<DialogComponent>().Close();
					}
					
					Item.Use((Player) Entity);
				}
			} else {
				timeSinceReady = 0;
			}
	
			if ((Input.WasPressed(Controls.Swap, controller) || (Input.Mouse.WheelDelta != 0 && stopped)) && Run.Depth > 0 && GetComponent<WeaponComponent>().Item != null) {
				stopped = false;
				Swap();
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

			if (Item != null && InGameState.Ready) {
				Audio.PlaySfx(Run.Depth == -2 ? Item.Data.WeaponType.GetSwapSfx() : Item.Data.WeaponType.GetPickupSfx());
			}
			
			if (Run.Depth == -2) {
				var dialog = GetComponent<DialogComponent>();
								
				dialog.Dialog.Str.ClearIcons();
				dialog.Dialog.Str.AddIcon(CommonAse.Ui.GetSlice(Controls.FindSlice(Controls.Use, false)));

				if (GamepadComponent.Current != null && GamepadComponent.Current.Attached) {
					dialog.Dialog.Str.AddIcon(CommonAse.Ui.GetSlice(Controls.FindSlice(Controls.Use, true)));
				}
								
				dialog.StartAndClose("control_2", 5);
			}
		}
	}
}