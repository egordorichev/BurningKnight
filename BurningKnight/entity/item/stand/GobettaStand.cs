using BurningKnight.state;
using Lens.entity;

namespace BurningKnight.entity.item.stand {
	public class GobettaStand : ShopStand {
		protected override string GetSprite() {
			return "gobetta_stand";
		}

		public override ItemPool GetPool() {
			return ItemPool.Gobetta;
		}

		protected override int CalculatePrice() {
			return (int) (base.CalculatePrice() * 0.7f);
		}

		protected override bool TryPay(Entity entity) {
			if (base.TryPay(entity)) {
				Run.AddScourge(true);
				return true;
			}

			return false;
		}
	}
}