using System;

namespace BurningKnight.entity.item {
	public class ItemInfo {
		public Chance Chance;
		public string Id;
		public Func<Item> Create;
		public float Warrior;
		public ItemType Type;

		public ItemInfo(string id, Func<Item> create, ItemType type = ItemType.Artifact, Chance chance = null) {
			Id = id;
			Create = create;
			Chance = chance ?? Chance.All();
			Type = type;
		}

		public bool Unlocked(string Key) {
			return true;
		}
	}
}