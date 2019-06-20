using System;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.item;
using Lens.entity;
using Lens.input;

namespace BurningKnight.entity.creature.player {
	public class ActiveItemComponent : ItemComponent {
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
			}
		}

		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Active;
		}
	}
}