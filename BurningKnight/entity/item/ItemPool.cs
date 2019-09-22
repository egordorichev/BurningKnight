using System.Collections.Generic;
using BurningKnight.assets.items;
using BurningKnight.util;
using Lens.util;

namespace BurningKnight.entity.item {
	public class ItemPool {
		public static Dictionary<string, ItemPool> ByName = new Dictionary<string, ItemPool>();
		public static ItemPool[] ById = new ItemPool[32];
		public static string[] Names = new string[32];

		public static readonly ItemPool Consumable = new ItemPool("consumable");
		public static readonly ItemPool Chest = new ItemPool("chest");
		public static readonly ItemPool Secret = new ItemPool("secret");
		public static readonly ItemPool Hat = new ItemPool("hat");
		public static readonly ItemPool Crate = new ItemPool("crate");
		public static readonly ItemPool StartingWeapon = new ItemPool("starting_weapon");
		public static readonly ItemPool Shop = new ItemPool("shop");
		public static readonly ItemPool Boss = new ItemPool("boss");
		public static readonly ItemPool ShopConsumable = new ItemPool("shop_consumable");

		private static int count;
		public static int Count => count;
		
		public readonly string Name;
		public readonly int Id;

		public ItemPool(string name) {
			if (count >= 32) {
				Log.Error($"Can not define item pool {name}, 32 pools were already defined");
				return;
			}
			
			Name = name;
			Id = count;

			ById[Id] = this;
			ByName[name] = this;
			Names[Id] = name;
			
			count++;
		}

		public int Apply(int pools, bool add = true) {
			return BitHelper.SetBit(pools, Id, add);
		}
		
		public bool Contains(int pools) {
			return BitHelper.IsBitSet(pools, Id);
		}
	}
}