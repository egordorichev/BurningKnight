using BurningKnight.assets.items;
using BurningKnight.entity.component;
using Lens.entity;

namespace BurningKnight.entity.item.use {
	public class GiveRandomPickupUse : ItemUse {
		public override void Use(Entity entity, Item item) {
			entity.GetComponent<InventoryComponent>().Pickup(Items.CreateAndAdd(Items.Generate(ItemPool.Consumable), entity.Area));
		}
	}
}