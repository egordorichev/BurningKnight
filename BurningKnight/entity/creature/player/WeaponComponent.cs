using BurningKnight.assets;
using BurningKnight.assets.achievements;
using BurningKnight.assets.input;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.level.entities;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.util;
using Lens.util.file;

namespace BurningKnight.entity.creature.player {
	public class WeaponComponent : ItemComponent {
		public bool Disabled;
		
		protected bool AtBack = true;
		private bool requestSwap;

		public void RequestSwap() {
			requestSwap = true;
		}
		
		public override void PostInit() {
			base.PostInit();

			if (Item != null && Run.Depth < 1) {
				Item.Done = true;
				Item = null;
			}

			UpdateItem = !AtBack;
		}

		protected override void OnItemSet(Item previous) {
			base.OnItemSet(previous);

			if (Item != null && Item.Scourged) {
				Achievements.Unlock("bk:scourged_weapon");
			}
		}

		public void Render(bool shadow, int offset) {
			if (!Disabled && Item != null && Item.Renderer != null) {
				if (!shadow) {
					var sh = Shaders.Item;
					Shaders.Begin(sh);
					sh.Parameters["time"].SetValue(Engine.Time * 0.1f);
					sh.Parameters["size"].SetValue(ItemGraphicsComponent.FlashSize);
				}

				Item.Renderer.Render(AtBack, Engine.Instance.State.Paused, Engine.Delta, shadow, offset);

				if (!shadow) {
					Shaders.End();
				}
			}
		}
		
		protected override bool ShouldReplace(Item item) {
			return (Run.Depth > 0 || !AtBack) && item.Type == ItemType.Weapon && !Disabled;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (requestSwap) {
				Swap();
				requestSwap = false;
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is ItemAddedEvent ev) {
				if (!Disabled && ev.Component == this) {
					ev.Old?.Drop();
					ev.Item?.Pickup();
					
					if (ev.Item != null && ev.Old == null && AtBack) {
						if (GlobalSave.IsTrue("control_swap")) {
							Entity.GetComponent<ActiveWeaponComponent>().requestSwap = InGameState.Ready;
						} else {
							var dialog = GetComponent<DialogComponent>();
								
							dialog.Dialog.Str.ClearIcons();
							dialog.Dialog.Str.AddIcon(CommonAse.Ui.GetSlice(Controls.FindSlice(Controls.Swap, false)));

							if (GamepadComponent.Current != null && GamepadComponent.Current.Attached) {
								dialog.Dialog.Str.AddIcon(CommonAse.Ui.GetSlice(Controls.FindSlice(Controls.Swap, true)));
							}
								
							dialog.StartAndClose("control_5", 5);
						}
					}
				}
			}

			return base.HandleEvent(e);
		}

		protected void Swap() {
			if (GlobalSave.IsFalse("control_swap")) {
				GlobalSave.Put("control_swap", true);
				Entity.GetComponent<DialogComponent>().Close();
			}
			
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

				if (!AtBack) {
					component.Item?.PutAway();
					Item?.TakeOut();
					
					Audio.PlaySfx(Item == null ? "swap" : Item.Data.WeaponType.GetSwapSfx());
				} else {
					Log.Error("Swap is called from not active weapon component");
				}
			}
		}

		public override void Load(FileReader stream) {
			base.Load(stream);
			Disabled = stream.ReadBoolean();
		}

		public override void Save(FileWriter stream) {
			base.Save(stream);
			stream.WriteBoolean(Disabled);
		}
	}
}