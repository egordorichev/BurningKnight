using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.creature.player;
using Lens.entity;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.level.entities.chest {
	public class GoldChest : Chest {
		protected byte KeysRequired = 1;

		public override string GetSprite() {
			return "gold_chest";
		}

		public override string GetPool() {
			return "bk:gold_chest";
		}

		protected override bool TryOpen(Entity entity) {
			if (!entity.TryGetComponent<ConsumablesComponent>(out var c) || c.Keys < KeysRequired) {
				return false;
			}

			c.Keys -= KeysRequired;
			return true;
		}

		protected override void SpawnDrops() {
			if (!Empty && Rnd.Chance(2.5f)) {
				var chest = Rnd.Chance(60) ? (Chest) new WoodenChest {
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