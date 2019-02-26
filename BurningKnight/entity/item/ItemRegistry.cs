using System;
using System.Collections.Generic;

namespace BurningKnight.entity.item {
	public static class ItemRegistry {
		public static Dictionary<string, Pair> Items = new Dictionary<string, Pair>();

		static ItemRegistry() {
			foreach (var pair in Pairs) {
				Items[pair.Id] = pair;
			}
		}

		public static bool Check(Quality a, Quality q) {
			if (q == Quality.Any || a == Quality.Any) {
				return true;
			}

			switch (q) {
				case Quality.Wooden: {
					return a == Quality.Wooden || a == Quality.WoodenPlus;
				}

				case Quality.Iron: {
					return a == Quality.Iron || a == Quality.IronPlus || a == Quality.WoodenPlus;
				}

				case Quality.Golden: {
					return a == Quality.Golden || a == Quality.WoodenPlus || a == Quality.IronPlus;
				}

				case Quality.WoodenPlus: {
					return true;
				}

				case Quality.IronPlus: {
					return a == Quality.Iron || a == Quality.IronPlus || a == Quality.Golden;
				}

				default: {
					return false;
				}
			}
		}

		public class Pair {
			public bool Busy;
			public float Chance;
			public int Cost;
			public string Id;
			public float Mage;
			public Quality Quality;
			public float Ranged;
			public bool Shown;
			public Type Type;
			public string Unlock;
			public float Warrior;

			public Pair(string Id, Type Type, float Chance, float Warrior, float Mage, float Ranged, Quality Quality) 
				: this(Id, Type, Chance, Warrior, Mage, Ranged, Quality, 0, null) {
			}

			public Pair(string Id, Type Type, float Chance, float Warrior, float Mage, float Ranged, Quality Quality, int Cost, string Unlock) {
				this.Id = Id;
				this.Type = Type;
				this.Chance = Chance;
				this.Warrior = Warrior;
				this.Mage = Mage;
				this.Ranged = Ranged;
				this.Quality = Quality;
				this.Unlock = Unlock;
				this.Cost = Cost;
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
		
		public static Pair[] Pairs = new [] {
			new Pair("test", typeof(Item), 0, 0, 0, 0, Quality.Any)
		};
	}
}