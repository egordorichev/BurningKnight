using BurningKnight.entity.creature.player;
using BurningKnight.entity.item;
using BurningKnight.entity.lamp;
using BurningKnight.util;
using Lens.util.math;

namespace BurningKnight.entity.component {
	public class LampComponent : ItemComponent {
		public Lamp Lamp;
		
		protected override void OnItemSet() {
			Item?.Use(Entity);

			if (Lamp != null) {
				Lamp.Done = true;
			}
			
			if (Item != null) {
				Lamp = new Lamp {
					Owner = (Player) Entity,
					Item = Item
				};
				
				Entity.Area.Add(Lamp);
				Lamp.Center = Entity.Center;
				AnimationUtil.Poof(Lamp.Center);
			}
		}

		protected override bool ShouldReplace(Item item) {
			return item.Type == ItemType.Lamp;
		}
	}
}