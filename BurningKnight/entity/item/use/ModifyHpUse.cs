using BurningKnight.entity.component;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class ModifyHpUse : ItemUse {
		public int Amount;

		public ModifyHpUse(int amount) {
			Amount = amount;
		}
		
		public void Use(Entity entity, Item item) {
			entity.GetComponent<HealthComponent>().ModifyHealth(Amount, entity);
		}
	}
}