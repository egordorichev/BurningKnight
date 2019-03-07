using BurningKnight.entity.component;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class ModifyMaxHpUse : ItemUse {
		public int Amount;

		public ModifyMaxHpUse(int amount) {
			Amount = amount * 2;
		}
		
		public void Use(Entity entity, Item item) {
			var component = entity.GetComponent<HealthComponent>();

			component.MaxHealth += Amount;
			
			if (Amount > 0) {
				component.ModifyHealth(Amount, entity);				
			}
		}
	}
}