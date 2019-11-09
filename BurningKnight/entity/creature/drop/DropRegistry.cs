using System;
using System.Collections.Generic;
using Lens.lightJson;

namespace BurningKnight.entity.creature.drop {
	public static class DropRegistry {
		public static Dictionary<string, DropInfo> Defined = new Dictionary<string, DropInfo>();

		static DropRegistry() {
			Define<AnyDrop>("any", AnyDrop.RenderDebug);
			Define<EmptyDrop>("empty", EmptyDrop.RenderDebug);
			Define<OneOfDrop>("one", OneOfDrop.RenderDebug);
			Define<PoolDrop>("pool", PoolDrop.RenderDebug);
			Define<SimpleDrop>("simple", SimpleDrop.RenderDebug);
			Define<SingleDrop>("single", SingleDrop.RenderDebug);
		}
		
		public static void Define<T>(string id, Action<JsonValue> render) where T : Drop {
			Defined[id] = new DropInfo {
				Render = render,
				Id = id,
				Type = typeof(T)
			};
		}
	}
}