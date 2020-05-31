using BurningKnight.assets.particle.custom;
using BurningKnight.entity.events;
using BurningKnight.util;
using Lens.assets;
using Lens.entity;
using Lens.lightJson;
using Lens.util.math;

namespace BurningKnight.entity.item.use {
	public class BlockDamageUse : ItemUse {
		private float chance;

		public override bool HandleEvent(Event e) {
			if (e is HealthModifiedEvent hme) {
				if (hme.Amount < 0 && hme.Who == Item.Owner && Rnd.Chance(chance)) {
					hme.Amount = 0;

					if (Item.Id == "bk:cats_ear") {
						TextParticle.Add(Item.Owner, "Sick dodge!");
					}
					
					return true;
				}
			}
			
			return base.HandleEvent(e);
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);
			chance = settings["chance"].Number(10);
		}

		public static void RenderDebug(JsonValue root) {
			root.InputFloat("Chance", "chance", 10);
		}
	}
}