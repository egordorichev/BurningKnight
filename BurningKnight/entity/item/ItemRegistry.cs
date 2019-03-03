using System.Collections.Generic;
using BurningKnight.entity.item.use;

namespace BurningKnight.entity.item {
	public static class ItemRegistry {
		private static ItemInfo[] infos = {
			new ItemInfo("health_potion", () => new Item(new ModifyHpUse(30)), new Chance.All(1)),
			
			new ItemInfo("random_potion", () => new Item(new RandomUse(
				new ModifyHpUse(30), new ModifyHpUse(-30)
			)), new Chance.All(1))
		};
		
		public static Dictionary<string, ItemInfo> Items = new Dictionary<string, ItemInfo>();

		static ItemRegistry() {
			foreach (var pair in infos) {
				Items[pair.Id] = pair;
			}
		}

		public static void Add(ItemInfo info) {
			Items[info.Id] = info;
		}

		public static void Remove(string id) {
			Items.Remove(id);
		}
	}
}