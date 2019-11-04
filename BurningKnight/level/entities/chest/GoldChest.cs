using BurningKnight.entity.component;
using BurningKnight.entity.creature.drop;
using BurningKnight.entity.creature.player;
using Lens.entity;
using Lens.util.math;

namespace BurningKnight.level.entities.chest {
	public class GoldChest : Chest {
		protected byte KeysRequired = 1;
		
		public GoldChest() {
			if (Sprite == null) {
				Sprite = "gold_chest";
			}
		}

		public override void AddComponents() {
			base.AddComponents();
			DefineDrops();
		}

		protected virtual void DefineDrops() {
			var drops = GetComponent<DropsComponent>();
			
			drops.Add(new OneOfDrop(
				new SingleDrop("bk:halo"),
				new SingleDrop("bk:wings"),
				new SingleDrop("bk:potatoo")	
			));
		}

		protected override bool TryOpen(Entity entity) {
			if (!entity.TryGetComponent<ConsumablesComponent>(out var c) || c.Keys < KeysRequired) {
				return false;
			}

			c.Keys -= KeysRequired;
			return true;
		}

		protected override void SpawnDrops() {
			if (Random.Chance(5)) {
				var chest = Random.Chance(60) ? (Chest) new WoodenChest {
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