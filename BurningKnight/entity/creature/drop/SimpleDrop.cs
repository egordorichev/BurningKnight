using System.Collections.Generic;
using Lens.util.math;

namespace BurningKnight.entity.creature.drop {
	public class SimpleDrop : Drop {
		public string[] Items;
		public int Min = 1;
		public int Max = 1;

		public SimpleDrop(float chance, int min, int max, params string[] items) {
			Chance = chance;
			Min = min;
			Max = max;
			Items = items;
		}
		
		public SimpleDrop() {
			
		}
		
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