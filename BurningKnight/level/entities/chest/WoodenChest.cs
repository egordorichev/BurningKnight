using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using Lens.util.math;

namespace BurningKnight.level.entities.chest {
	public class WoodenChest : Chest {
		/*
		 * todo:
		 * chance for a smaller wooden chest
		 */

		public WoodenChest() {
			Sprite = "wooden_chest";
		}

		protected override void DefineDrops() {
			var drops = GetComponent<DropsComponent>();

			drops.Add(new OneOfDrop(
				new AnyDrop(
					new SimpleDrop(0.7f, 1, 2, "bk:key"),
					new SimpleDrop(0.5f, 1, 2, "bk:bomb"),
					new SimpleDrop(0.5f, 1, 2, "bk:troll_bomb"),
					new SimpleDrop(0.6f, 1, 4, "bk:coin")
				),
				
				new AnyDrop(
					new SingleDrop("bk:halo", 1f)
				)
			));
		}
		
		protected override void SpawnDrops() {
			if (Random.Chance(10)) {
				var chest = Random.Chance() ? (Chest) new WoodenChest {
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