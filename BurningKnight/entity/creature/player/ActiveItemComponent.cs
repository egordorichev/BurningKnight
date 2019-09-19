using System;
using BurningKnight.assets.input;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using BurningKnight.state;
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
				if (Item != null && Item.UseTime > 0) {
					Item.Delay = Math.Max(0, Item.Delay - 1);
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Item != null && Input.WasPressed(Controls.Active, GetComponent<GamepadComponent>().Controller)) {
				Item.Use((Player) Entity);
				Entity.GetComponent<AudioEmitterComponent>().EmitRandomized("active_item");

				if (Math.Abs(Item.UseTime) <= 0.01f) {
					Item.Done = true;
				}
			}
		}

		public void Clear() {
			Item = null;
		}

		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Active;
		}
	}
}