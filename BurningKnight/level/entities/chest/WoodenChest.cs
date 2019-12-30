using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using Lens.util.math;

namespace BurningKnight.level.entities.chest {
	public class WoodenChest : Chest {
		protected override string GetSprite() {
			return Events.XMas ? "xmas_chest" : "wooden_chest";
		}

		protected override void DefineDrops() {
			GetComponent<DropsComponent>().Add("bk:wooden_chest");
		}
		
		protected override void SpawnDrops() {
			if (Rnd.Chance(5)) {
				var chest = Rnd.Chance() ? (Chest) new WoodenChest {
					Scale = Scale * 0.9f
				} : (Chest) new GoldChest {
					Scale = Scale * 0.9f
				};

				Area.Add(chest);
				chest.TopCenter = BottomCenter;
				
				return;
			}
			
			base.SpawnDrops();
		}
	}
}