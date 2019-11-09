using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.entity.item;
using Lens.lightJson;
using Lens.util.math;

namespace BurningKnight.entity.creature.drop {
	public class PoolDrop : Drop {
		public ItemPool Pool;
		public int Min;
		public int Max;

		public PoolDrop() {
			
		}
		
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
					list.Add(Items.Generate(Pool));
				}
			}

			return list;
		}

		public override string GetId() {
			return "pool";
		}

		public override void Load(JsonValue root) {
			base.Load(root);

			Min = root["min"].Int(1);
			Max = root["max"].Int(1);
			Pool = ItemPool.ById[root["pool"].Int(1)];
		}

		public override void Save(JsonValue root) {
			base.Save(root);

			root["min"] = Min;
			root["max"] = Max;
			root["pool"] = Pool.Id;
		}
	}
}