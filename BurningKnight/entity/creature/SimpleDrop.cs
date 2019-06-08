using System.Collections.Generic;
using Lens.util.math;

namespace BurningKnight.entity.creature {
	public class SimpleDrop : Drop {
		public string[] Items;
		public int Min = 1;
		public int Max = 1;
		
		public override List<string> GetItems() {
			var items = base.GetItems();

			if (Items != null) {
				for (var i = 0; i < Random.Int(Min, Max + 1); i++) {
					items.AddRange(Items);
				}
			}
			
			return items;
		}
	}
}