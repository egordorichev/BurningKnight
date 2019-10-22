using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.mod;
using Lens.lightJson;

namespace BurningKnight.entity.item.renderer {
	public static class RendererRegistry {
		public static Dictionary<string, Action<string, JsonValue, JsonValue>> DebugRenderers = new Dictionary<string, Action<string, JsonValue, JsonValue>>();
		public static Dictionary<string, Type> Renderers = new Dictionary<string, Type>();

		public static ItemRenderer Create(string id) {
			if (!Renderers.TryGetValue(id, out var renderer)) {
				return null;
			}

			return (ItemRenderer) Activator.CreateInstance(renderer);
		}


		public static void Register<T>(Mod mod, Action<string, JsonValue, JsonValue> renderer = null) where T : ItemRenderer {
			var type = typeof(T);
			var name = type.Name;
			var id = $"{mod?.Prefix ?? Mods.BurningKnight}:{(name.EndsWith("Renderer") ? name.Substring(0, name.Length - 8) : name)}";

			Renderers[id] = type;

			if (renderer != null) {
				DebugRenderers[id] = renderer;
			}
		}

		private static void Register<T>(Action<string, JsonValue, JsonValue> renderer = null) where T : ItemRenderer {
			Register<T>(null, renderer);
		}

		static RendererRegistry() {
			Register<AngledRenderer>(AngledRenderer.RenderDebug);
			Register<MovingAngledRenderer>(MovingAngledRenderer.RenderDebug);
			Register<StickRenderer>(StickRenderer.RenderDebug);
		}
	}
}