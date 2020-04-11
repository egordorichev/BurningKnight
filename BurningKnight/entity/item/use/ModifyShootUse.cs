using BurningKnight.entity.events;
using BurningKnight.util;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class ModifyShootUse : ItemUse {
		private int amount;

		public override bool HandleEvent(Event e) {
			if (e is PlayerShootEvent pse) {
				pse.Times += amount;
				pse.Accurate = true;
			}
		
			return base.HandleEvent(e);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			amount = settings["amount"].Int(1);
		}

		public static void RenderDebug(JsonValue root) {
			root.InputInt("Amount", "amount", 1);
		}
	}
}