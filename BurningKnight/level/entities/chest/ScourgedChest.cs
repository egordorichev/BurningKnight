using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.entity.component;
using BurningKnight.entity.item;
using BurningKnight.state;
using Lens.entity;
using Lens.util.timer;

namespace BurningKnight.level.entities.chest {
	public class ScourgedChest : Chest, DropModifier {
		protected override string GetSprite() {
			return "scourged_chest";
		}

		protected override void DefineDrops() {
			GetComponent<DropsComponent>().Add("bk:scourged_chest");
		}

		protected override bool TryOpen(Entity entity) {
			var item = Scourge.GenerateItemId();

			if (item != null) {
				Timer.Add(() => entity.GetComponent<InventoryComponent>().Pickup(Items.CreateAndAdd(item, entity.Area)), 0.3f);
			}

			return true;
		}

		public void ModifyDrops(List<Item> drops) {
			foreach (var d in drops) {
				d.Scourged = true;
			}
		}
	}
}