using System.Collections.Generic;

namespace BurningKnight.entity.creature.drop {
	public class SingleDrop : Drop {
		public string Item;

		public SingleDrop(string id, float chance) {
			Item = id;
			Chance = chance;
		}

		public override List<string> GetItems() {
			var items = base.GetItems();
			items.Add(Item);
			return items;
		}
	}
}