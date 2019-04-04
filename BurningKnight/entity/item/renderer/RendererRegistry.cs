using System;
using System.Collections.Generic;
using BurningKnight.assets;

namespace BurningKnight.entity.item.renderer {
	public static class RendererRegistry {
		private static Dictionary<string, Type> renderers = new Dictionary<string, Type>();

		public static ItemRenderer Create(string id) {
			if (!renderers.TryGetValue(id, out var renderer)) {
				return null;
			}

			return (ItemRenderer) Activator.CreateInstance(renderer);
		}

		
		public static void Register<T>(Mod mod) where T : ItemRenderer {
			var type = typeof(T);
			var name = type.Name;
			var id = $"{mod?.GetPrefix() ?? Mods.BurningKnight}:{(name.EndsWith("Renderer") ? name.Substring(0, name.Length - 8) : name)}";

			renderers[id] = type;
		}

		private static void Register<T>() where T : ItemRenderer {
			Register<T>(null);
		}

		static RendererRegistry() {
			Register<AngledRenderer>();
			Register<MovingAngledRenderer>();
		}
	}
}