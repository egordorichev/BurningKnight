using System;

namespace BurningKnight.entity.item {
	public class ItemInfo {
		public Chance Chance;
		public string Id;
		public Func<Item> Create;
		public float Warrior;

		public ItemInfo(string id, Func<Item> create, Chance chance) {
			Id = id;
			Create = create;
			Chance = chance;
		}

		public bool Unlocked(string Key) {
			return true;
		}
	}
}