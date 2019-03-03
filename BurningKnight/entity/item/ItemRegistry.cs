using System;
using System.Collections.Generic;
using BurningKnight.entity.item.use;

namespace BurningKnight.entity.item {
	public static class ItemRegistry {
		public static Dictionary<string, Pair> Items = new Dictionary<string, Pair>();

		static ItemRegistry() {
			foreach (var pair in Pairs) {
				Items[pair.Id] = pair;
			}
		}

		public class Pair {
			public Chance Chance;
			public string Id;
			public Func<Item> Create;
			public float Warrior;

			public Pair(string id, Func<Item> create, Chance chance) {
				Id = id;
				Create = create;
				Chance = chance;
			}

			public bool Unlocked(string Key) {
				return true;
			}
		}

		public enum Quality {
			Wooden,
			Iron,
			Golden,
			WoodenPlus,
			IronPlus,
			Any
		}
		
		public static Pair[] Pairs = {
			new Pair("health_potion", () => new Item(new ModifyHpUse(30)), new Chance.All(1)),
			
			new Pair("random_potion", () => new Item(new RandomUse(
				new ModifyHpUse(30), new ModifyHpUse(-30)
			)), new Chance.All(1))
		};
	}
}