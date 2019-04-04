using BurningKnight.entity.item;
using Lens.lightJson;

namespace BurningKnight.assets.items {
	public class ItemData {
		public JsonValue Uses;
		public JsonValue Renderer;
		
		public string Id;
		public float UseTime;
		public ItemType Type;
	}
}