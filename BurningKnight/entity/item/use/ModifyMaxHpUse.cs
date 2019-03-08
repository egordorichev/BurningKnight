using BurningKnight.entity.component;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class ModifyMaxHpUse : ItemUse {
		public int Amount;
		public bool GiveHp;

		public ModifyMaxHpUse(int amount, bool giveHp = true) {
			Amount = amount * 2;
			GiveHp = giveHp;
		}
		
		public void Use(Entity entity, Item item) {
			var component = entity.GetComponent<HealthComponent>();

			component.MaxHealth += Amount;
			
			if (GiveHp && Amount > 0) {
				component.ModifyHealth(Amount, entity);				
			}
		}
	}
}