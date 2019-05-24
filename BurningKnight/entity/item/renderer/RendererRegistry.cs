using System;
using System.Collections.Generic;
using BurningKnight.assets;
using Lens.lightJson;

namespace BurningKnight.entity.item.renderer {
	public static class RendererRegistry {
		public static Dictionary<string, Action<JsonValue>> Renderers = new Dictionary<string, Action<JsonValue>>();
		private static Dictionary<string, Type> renderers = new Dictionary<string, Type>();

		public static ItemRenderer Create(string id) {
			if (!renderers.TryGetValue(id, out var renderer)) {
				return null;
			}

			return (ItemRenderer) Activator.CreateInstance(renderer);
		}


		public static void Register<T>(Mod mod, Action<JsonValue> renderer = null) where T : ItemRenderer {
			var type = typeof(T);
			var name = type.Name;
			var id = $"{mod?.GetPrefix() ?? Mods.BurningKnight}:{(name.EndsWith("Renderer") ? name.Substring(0, name.Length - 8) : name)}";

			renderers[id] = type;

			if (renderer != null) {
				Renderers[id] = renderer;
			}
		}

		private static void Register<T>(Action<JsonValue> renderer = null) where T : ItemRenderer {
			Register<T>(null, renderer);
		}

		static RendererRegistry() {
			Register<AngledRenderer>(AngledRenderer.RenderDebug);
			Register<MovingAngledRenderer>(MovingAngledRenderer.RenderDebug);
		}
	}
}