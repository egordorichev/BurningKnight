using BurningKnight.entity.item;
using Lens.lightJson;

namespace BurningKnight.assets.items {
	public class ItemData {
		public JsonValue Root;
		public JsonValue Uses;
		public JsonValue Renderer;

		public bool AutoPickup;
		public bool Automatic;
		public string Animation;
		public string Id;
		public float UseTime;
		public ItemType Type;
		public Chance Chance;
		public int Pools;
		public bool Single = true;
		public bool Lockable;
	}
}