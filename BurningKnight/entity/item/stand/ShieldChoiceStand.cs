using BurningKnight.assets.items;
using BurningKnight.entity.component;
using Lens.entity;

namespace BurningKnight.entity.item.stand {
	public class ShieldChoiceStand : HealChoiceStand {
		protected override string GetSprite() {
			return "shield_stand";
		}

		protected override string GetSfx() {
			return "item_shield";
		}

		protected override void Heal(Entity entity) {
			for (var i = 0; i < 3; i++) {
				entity.GetComponent<InventoryComponent>().Pickup(Items.CreateAndAdd("bk:shield", entity.Area));
			}
		}
	}
}