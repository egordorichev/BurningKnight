using System.Collections.Generic;

namespace BurningKnight.entity.creature {
	public class SimpleDrop : Drop {
		public string[] Items;

		public override List<string> GetItems() {
			var items = base.GetItems();

			if (Items != null) {
				items.AddRange(Items);
			}
			
			return items;
		}
	}
}