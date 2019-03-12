using BurningKnight.entity.creature.player;
using Lens.entity;

namespace BurningKnight.entity.level.entities {
	public class GoldLock : Lock {
		protected override bool TryToConsumeKey(Entity entity) {
			if (!entity.TryGetComponent<ConsumablesComponent>(out var component)) {
				return false;
			}

			if (component.Keys > 0) {
				component.Keys--;
				return true;
			}
			
			return false;
		}
	}
}