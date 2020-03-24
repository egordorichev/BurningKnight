using System;
using BurningKnight.entity.creature.player;
using Lens.entity;

namespace BurningKnight.entity.item.stand {
	public class BoxyStand : CustomStand {
		protected override string GetIcon() {
			return "deal_key";
		}

		protected override string GetSprite() {
			return "boxy_stand";
		}

		public override ItemPool GetPool() {
			return ItemPool.Roger;
		}

		protected override int CalculatePrice() {
			return (int) Math.Max(1, PriceCalculator.GetModifier(Item) * 2);
		}

		protected override bool TryPay(Entity entity) {
			if (!entity.TryGetComponent<ConsumablesComponent>(out var component)) {
				return false;
			}

			if (component.Keys < Price) {
				return false;
			}

			component.Keys -= Price;
			return true;
		}
		
		protected override bool HasEnoughToPay(Entity p) {
			return p.GetComponent<ConsumablesComponent>().Keys >= Price;
		}
	}
}