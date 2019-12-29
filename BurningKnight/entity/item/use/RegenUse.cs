using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.util;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class RegenUse : ItemUse {
		private int speed;
		private int count;
		
		public override bool HandleEvent(Event e) {
			if (e is KilledEvent) {
				count++;
				
				if (count >= speed) {
					count = 0;
					Item.Owner.GetComponent<HealthComponent>().ModifyHealth(1, Item);
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			speed = settings["speed"].Int(10);
		}

		public static void RenderDebug(JsonValue root) {
			root.InputInt("Speed", "speed", 10);
		}
	}
}