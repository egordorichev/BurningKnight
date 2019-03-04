using System;
using System.Collections.Generic;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.item.use;
using Lens.entity.component.graphics;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.item {
	public static class ItemRegistry {
		public static Dictionary<string, ItemInfo> Items = new Dictionary<string, ItemInfo>();
		public static Dictionary<ItemType, List<ItemInfo>> ByType = new Dictionary<ItemType, List<ItemInfo>>();
		
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

			if (ByType.TryGetValue(info.Type, out var rooms)) {
				rooms.Add(info);
			} else {
				rooms = new List<ItemInfo>();
				rooms.Add(info);
				
				ByType[info.Type] = rooms;
			}
		}

		public static void Remove(string id) {
			if (!Items.TryGetValue(id, out var item)) {
				return;
			}
			
			Items.Remove(id);
			ByType[item.Type].Remove(item);
		}

		public static Item Create(string id) {
			if (!Items.TryGetValue(id, out var info)) {
				return null;
			}

			return CreateFrom(info);
		}

		public static Item CreateFrom(ItemInfo info) {
			var item = info.Create();
			
			item.Id = info.Id;
			// todo: custom item render component
			item.AddComponent(new ImageComponent(item.Id));

			return item;
		}

		public static Item Generate(ItemType type, PlayerClass c = PlayerClass.Any) {
			if (!ByType.TryGetValue(type, out var types)) {
				return null;
			}
			
			float sum = 0;

			foreach (var chance in types) {
				sum += chance.Chance.Calculate(c);
			}

			float value = Random.Float(sum);
			sum = 0;

			foreach (var t in types) {
				sum += t.Chance.Calculate(c);

				if (value < sum) {
					return CreateFrom(t);
				}
			}

			return null;
		}
	}
}