using System;
using Lens.lightJson;

namespace BurningKnight.entity.creature.drop {
	public struct DropInfo {
		public string Id;
		public Type Type;
		public Action<JsonValue> Render;
	}
}