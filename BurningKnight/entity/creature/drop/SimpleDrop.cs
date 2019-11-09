using System.Collections.Generic;
using Lens.lightJson;
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
					foreach (var item in Items) {
						if (assets.items.Items.ShouldAppear(item)) {
							items.Add(item);
						}
					}
				}
			}
			
			return items;
		}

		public override string GetId() {
			return "simple";
		}

		public override void Load(JsonValue root) {
			base.Load(root);
			
			Min = root["min"].Int(1);
			Max = root["max"].Int(1);

			if (root["items"].IsJsonArray) {
				var items = root["items"].AsJsonArray;
				Items = new string[items.Count];

				for (var i = 0; i < Items.Length; i++) {
					Items[i] = items[i].AsString;
				}
			}
		}

		public override void Save(JsonValue root) {
			base.Save(root);

			var items = new JsonArray();

			foreach (var item in Items) {
				items.Add(item);
			}
			
			root["min"] = Min;
			root["max"] = Max;
			root["items"] = items;
		}
	}
}