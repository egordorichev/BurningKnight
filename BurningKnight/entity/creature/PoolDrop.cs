using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.entity.item;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.creature {
	public class PoolDrop : Drop {
		public ItemPool Pool;
		public int Min;
		public int Max;
		
		public PoolDrop(ItemPool pool, float chance, int min, int max) {
			Pool = pool;
			Chance = chance;
			Min = min;
			Max = max;
		}

		public override List<string> GetItems() {
			var list = new List<string>();

			if (Random.Float() <= Chance) {
				for (var i = 0; i < Random.Int(Min, Max + 1); i++) {
					var it = Items.Generate(Pool);
					Log.Error(it);
					list.Add(it);
				}
			}

			return list;
		}
	}
}