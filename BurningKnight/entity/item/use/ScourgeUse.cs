using BurningKnight.assets.particle.custom;
using BurningKnight.state;
using BurningKnight.util;
using Lens.assets;
using Lens.entity;
using Lens.lightJson;

namespace BurningKnight.entity.item.use {
	public class ScourgeUse : ItemUse {
		private int amount;
		
		public override void Use(Entity entity, Item item) {
			base.Use(entity, item);

			for (var i = 0; i < amount; i++) {
				Run.AddScourge();
			}
			
			TextParticle.Add(entity, Locale.Get("scourge"), amount, true);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			amount = settings["amount"].Int(1);
		}
		
		public static void RenderDebug(JsonValue root) {
			root.InputInt("Amount", "amount");
		}
	}
}