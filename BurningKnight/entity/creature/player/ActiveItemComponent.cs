using System;
using BurningKnight.assets.input;
using BurningKnight.debug;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.ui.dialog;
using Lens.entity;
using Lens.input;

namespace BurningKnight.entity.creature.player {
	public class ActiveItemComponent : ItemComponent {
		public override void PostInit() {
			base.PostInit();

			if (Item != null && Run.Depth < 1) {
				Item.Done = true;
				Item = null;
			}
		}

		public override bool HandleEvent(Event e) {
			if (e is RoomClearedEvent) {
				if (Item != null && Item.UseTime > 0.02f) {
					Item.Delay = Math.Max(Item.Delay - 1, 0f);
				}
			}
			
			return base.HandleEvent(e);
		}

		protected override void OnItemSet(Item previous) {
			base.OnItemSet(previous);
			
			if (Run.Depth > 0 && GlobalSave.IsFalse("control_active") && GetComponent<DialogComponent>().Dialog?.Str != null) {
				GetComponent<DialogComponent>().Dialog.Str.SetVariable("ctrl", Controls.Find(Controls.Active, GamepadComponent.Current != null, true));
				Entity.GetComponent<DialogComponent>().StartAndClose("control_6", 5);
			}
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Run.Depth > 0 && Item != null && !Item.Done && Input.WasPressed(Controls.Active, GetComponent<GamepadComponent>().Controller)) {
				if (Item.Use((Player) Entity)) {
					if (CheatWindow.InfiniteActive) {
						Item.Delay = 0;
					}
					
					if (Run.Depth > 0 && GlobalSave.IsFalse("control_active")) {
						Entity.GetComponent<DialogComponent>().Close();
						GlobalSave.Put("control_active", true);
					}
					
					Entity.GetComponent<AudioEmitterComponent>().EmitRandomized("active_item");

					if (Item.SingleUse && !CheatWindow.InfiniteActive) {
						Item.Done = true;
					}
				}
			}
		}

		public void Clear() {
			Item = null;
		}

		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Active;
		}
		
		public bool IsFullOrEmpty() {
			return Item == null || Item.Delay <= 0.01f;
		}

		public void Charge(int amount) {
			if (Item != null && Item.UseTime > 0.02f) {
				Item.Delay = Math.Max(Item.Delay - amount, 0f);
			}
		}
	}
}