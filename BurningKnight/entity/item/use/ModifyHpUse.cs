using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;

namespace BurningKnight.entity.item.use {
	public class ModifyHpUse : ItemUse {
		public int Amount;

		public ModifyHpUse(int amount) {
			Amount = amount;
		}
		
		public void Use(Player player, Item item) {
			player.GetComponent<HealthComponent>().ModifyHealth(Amount, player);
		}
	}
}