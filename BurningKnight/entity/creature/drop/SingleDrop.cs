using System.Collections.Generic;

namespace BurningKnight.entity.creature.drop {
	public class SingleDrop : Drop {
		public string Item;

		public SingleDrop(string id, float chance = 1f) {
			Item = id;
			Chance = chance;
		}

		public override List<string> GetItems() {
			var items = base.GetItems();
			
			if (assets.items.Items.ShouldAppear(Item)) {
				items.Add(Item);
			}

			return items;
		}
	}
}