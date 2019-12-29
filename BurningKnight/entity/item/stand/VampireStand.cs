using BurningKnight.entity.component;
using BurningKnight.entity.events;
using Lens.entity;

namespace BurningKnight.entity.item.stand {
	public class VampireStand : CustomStand {
		private Entity payer;
		private Item takenItem;
		private int lastPrice;

		public override void Init() {
			base.Init();
			
			Subscribe<ItemUsedEvent>();
			Subscribe<ItemAddedEvent>();
		}

		protected override string GetIcon() {
			return "deal_hp";
		}

		protected override string GetSprite() {
			return "vampire_stand";
		}

		public override ItemPool GetPool() {
			return ItemPool.Vampire;
		}

		protected override int CalculatePrice() {
			return (int) (PriceCalculator.GetModifier(Item) * 1.5f);
		}

		protected override bool TryPay(Entity entity) {
			if (!entity.TryGetComponent<HealthComponent>(out var component)) {
				return false;
			}

			if (component.Health < Price * 2) {
				return false;
			}
			
			
			payer = entity;
			lastPrice = Price * 2;
			takenItem = Item;

			return true;
		}
		
		public override bool HandleEvent(Event e) {
			if ((e is ItemUsedEvent ite && ite.Who == payer && ite.Item == takenItem) || (e is ItemAddedEvent iae && iae.Who == payer && iae.Item == takenItem)) {
				var component = payer.GetComponent<HealthComponent>();
				 
				component.ModifyHealth(-lastPrice, this, DamageType.Custom);
				
				payer = null;
				takenItem = null;
			}
			
			return base.HandleEvent(e);
		}
		
		protected override bool HasEnoughToPay(Entity p) {
			return p.GetComponent<HealthComponent>().Health >= Price * 2;
		}
	}
}