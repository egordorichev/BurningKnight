using System;
using BurningKnight.assets;
using BurningKnight.assets.input;
using BurningKnight.entity.buff;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using BurningKnight.entity.item.use;
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

			var controller = GetComponent<InputComponent>();
			var data = controller.GamepadEnabled ? controller.GamepadData : null;

			if (!Disabled && Item != null) {
				var ready = Item.Delay <= 0.001f;

				if (ready) {
					if (timeSinceReady <= 0.001f && Item.Type == ItemType.Weapon) {
						var t = Item.Data.WeaponType;
						
						if (t == WeaponType.Ranged) {
							foreach (var u in Item.Uses) {
								if (u is SimpleShootUse s) {
									if (s.ReloadSfx) {
										Entity.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("item_shotgun_reload", 2, 0.8f);
									}

									break;
								}
							}	
						} else if (t == WeaponType.Melee) {
							Entity.GetComponent<AudioEmitterComponent>().EmitRandomized("item_sword_cooldown", 0.5f);
						}
					}
					
					timeSinceReady += dt;
				} else {
					timeSinceReady = 0;
				}
				
				var b = GetComponent<BuffsComponent>();
				
				if (b.Has<FrozenBuff>() || b.Has<CharmedBuff>() || GetComponent<StateComponent>().StateInstance is Player.RollState) {
					return;
				}
				
				if ((Input.WasPressed(Controls.Use, controller) || (data != null && (
					data.DPadDownCheck || data.DPadLeftCheck || data.DPadUpCheck || data.DPadRightCheck                                                  
				  ))) || ((Item.Automatic || timeSinceReady > 0.2f || (data != null && Input.IsDownOnController(Controls.Use, data))) && Input.IsDown(Controls.Use, controller) && ready)) {
				  
					if (!Entity.TryGetComponent<PlayerInputComponent>(out var d) || d.InDialog) {
						return;
					}

					if (GetComponent<StateComponent>().StateInstance is Player.SleepingState) {
						GetComponent<StateComponent>().Become<Player.IdleState>();
					}
					
					if (Run.Depth == -2) {
						GetComponent<DialogComponent>().Close();
					}

					if (b.Has<InvisibleBuff>()) {
						b.Remove<InvisibleBuff>();
					}
					
					Item.Use((Player) Entity);
					GetComponent<StatsComponent>().UsedWeaponInRoom = true;
				}
			} else {
				timeSinceReady = 0;
			}
	
			if ((Input.WasPressed(Controls.Swap, controller) || (Input.Mouse.WheelDelta != 0 && stopped)) && Run.Depth > 0 && GetComponent<WeaponComponent>().Item != null) {
				if (!GetComponent<InventoryComponent>().Busy) {
					stopped = false;
					Swap();
				}
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
				Audio.PlaySfx(Item.Data.WeaponType.GetSwapSfx());
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