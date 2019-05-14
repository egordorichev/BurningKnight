using System;
using System.Collections.Generic;

namespace BurningKnight.ui.imgui.node {
	public static class ImNodeRegistry {
		public static Dictionary<string, Type> Defined = new Dictionary<string, Type>();

		static ImNodeRegistry() {
			Define<ImTextNode>("text");
			Define<ImDialogNode>("dialog");
		}
		
		public static void Define<T>(string name) where T : ImNode {
			Defined[name] = typeof(T);
		}

		public static ImNode Create(string name) {
			if (!Defined.TryGetValue(name, out var type)) {
				return null;
			}

			return (ImNode) Activator.CreateInstance(type);
		}

		public static string GetName(ImNode node) {
			foreach (var d in Defined) {
				if (d.Value == node.GetType()) {
					return d.Key;
				}
			}

			return null;
		}
	}
}