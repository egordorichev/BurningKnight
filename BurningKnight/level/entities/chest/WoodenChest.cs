using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using Lens.util.math;

namespace BurningKnight.level.entities.chest {
	public class WoodenChest : Chest {
		public override string GetSprite() {
			return Events.XMas ? "xmas_chest" : "wooden_chest";
		}

		public override string GetPool() {
			return "bk:wooden_chest";
		}

		protected override void SpawnDrops() {
			if (!Empty && Rnd.Chance(5)) {
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