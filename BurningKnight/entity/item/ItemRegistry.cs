using System.Collections.Generic;
using BurningKnight.entity.item.use;
using Lens.entity.component.graphics;

namespace BurningKnight.entity.item {
	public static class ItemRegistry {
		public static Dictionary<string, ItemInfo> Items = new Dictionary<string, ItemInfo>();

		static ItemRegistry() {
			ItemInfo[] infos = {
				new ItemInfo("health_potion", () => new Item(new ModifyHpUse(30)), ItemType.Active),
			
				new ItemInfo("random_potion", () => new Item(new RandomUse(
					new ModifyHpUse(30), new ModifyHpUse(-30)
				)), ItemType.Active),
				
				new ItemInfo("bomb", () => new Item(new SpawnBombUse()), ItemType.Bomb)
			};

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

		public static Item Create(string id) {
			if (!Items.TryGetValue(id, out var info)) {
				return null;
			}

			var item = info.Create();
			item.Id = info.Id;
			
			// todo: custom item render component
			item.AddComponent(new ImageComponent(item.Id));
			
			return item;
		}

		public static Item Random(ItemType type) {
			
		}
	}
}