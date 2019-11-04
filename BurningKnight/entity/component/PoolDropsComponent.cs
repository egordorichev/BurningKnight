using BurningKnight.entity.creature;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.item;

namespace BurningKnight.entity.component {
	public class PoolDropsComponent : DropsComponent {
		public PoolDropsComponent(ItemPool pool, float chance, int min, int max) {
			Add(new PoolDrop(pool, chance, min, max));
		}
	}
}