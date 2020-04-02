using System;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.state;
using Lens.entity;
using Lens.util.timer;

namespace BurningKnight.entity.item.stand {
	public class GobettaStand : ShopStand {
		protected override string GetSprite() {
			return "gobetta_stand";
		}

		public override ItemPool GetPool() {
			return ItemPool.Gobetta;
		}

		protected override int CalculatePrice() {
			return (int) Math.Max(1, (base.CalculatePrice() * 0.5f));
		}

		protected override bool TryPay(Entity entity) {
			if (base.TryPay(entity)) {
				var scourge = Scourge.GenerateItemId();

				if (scourge != null) {
					Timer.Add(() => {
						entity.GetComponent<InventoryComponent>().Pickup(Items.CreateAndAdd(scourge, entity.Area));
					}, 2f);
				}
			
				return true;
			}

			return false;
		}
	}
}