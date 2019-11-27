using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.creature.player;
using Lens.entity;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.level.entities.chest {
	public class GoldChest : Chest {
		protected byte KeysRequired = 1;
		
		public GoldChest() {
			if (Sprite == null) {
				Sprite = "gold_chest";
			}
		}

		protected override void DefineDrops() {
			GetComponent<DropsComponent>().Add("bk:gold_chest");
		}

		protected override bool TryOpen(Entity entity) {
			if (!entity.TryGetComponent<ConsumablesComponent>(out var c) || c.Keys < KeysRequired) {
				return false;
			}

			c.Keys -= KeysRequired;
			return true;
		}

		protected override void SpawnDrops() {
			if (Rnd.Chance(5)) {
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