using BurningKnight.entity.creature.player;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class ModifyGoldHeartsUse : ItemUse {
		public int Amount;

		public ModifyGoldHeartsUse(int amount) {
			Amount = amount;
		}
		
		public void Use(Entity entity, Item item) {
			entity.GetComponent<HeartsComponent>().ModifyGoldHearts(Amount * 2, entity);
		}
	}
}