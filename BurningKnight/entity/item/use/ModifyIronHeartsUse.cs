using BurningKnight.entity.creature.player;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class ModifyIronHeartsUse : ItemUse {
		public int Amount;

		public ModifyIronHeartsUse(int amount) {
			Amount = amount;
		}
		
		public void Use(Entity entity, Item item) {
			entity.GetComponent<HeartsComponent>().ModifyIronHearts(Amount * 2, entity);
		}
	}
}