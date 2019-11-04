using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using Lens.entity;

namespace BurningKnight.level.entities.chest {
	public class RedChest : Chest {
		public RedChest() {
			Sprite = "red_chest";
		}

		protected override void DefineDrops() {
			var drops = GetComponent<DropsComponent>();
			
			drops.Add(new OneOfDrop(
				new SingleDrop("bk:broken_heart"),
				new SimpleDrop(1f, 3, 8, "bk:coin")
			));
		}

		protected override bool TryOpen(Entity entity) {
			if (!entity.TryGetComponent<HealthComponent>(out var h) || h.Health < 3) {
				return false;
			}

			h.ModifyHealth(-2, this);
			return true;
		}
	}
}