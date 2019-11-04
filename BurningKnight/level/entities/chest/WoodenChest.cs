using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;

namespace BurningKnight.level.entities.chest {
	public class WoodenChest : Chest {
		/*
		 * todo:
		 * chance for a smaller wooden chest
		 */

		public WoodenChest() {
			Sprite = "wooden_chest";
		}

		public override void AddComponents() {
			base.AddComponents();
			var drops = GetComponent<DropsComponent>();

			drops.Add(
				new OneOfDrop(
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
	}
}