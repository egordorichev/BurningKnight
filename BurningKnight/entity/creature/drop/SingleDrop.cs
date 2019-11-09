using System.Collections.Generic;
using Lens.lightJson;

namespace BurningKnight.entity.creature.drop {
	public class SingleDrop : Drop {
		public string Item;

		public SingleDrop() {
			
		}

		public SingleDrop(string id, float chance = 1f) {
			Item = id;
			Chance = chance;
		}

		public override List<string> GetItems() {
			var items = new List<string>();
			
			if (assets.items.Items.ShouldAppear(Item)) {
				items.Add(Item);
			}

			return items;
		}

		public override string GetId() {
			return "single";
		}

		public override void Load(JsonValue root) {
			base.Load(root);
			Item = root["item"].String("");
		}

		public override void Save(JsonValue root) {
			base.Save(root);
			root["item"] = Item;
		}

		public static void RenderDebug(JsonValue root) {
			
		}
	}
}