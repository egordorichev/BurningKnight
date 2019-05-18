using System;
using System.Collections.Generic;

namespace BurningKnight.ui.imgui.node {
	public static class ImNodeRegistry {
		public static Dictionary<string, Type> Defined = new Dictionary<string, Type>();

		static ImNodeRegistry() {
			Define<ImTextOutputNode>("Text output");
			Define<ImTextInputNode>("Text input");
			Define<ImOrdNode>("Ord");
			Define<ImDialogNode>("Dialog");
			Define<ImChoiceNode>("Choice");
		}
		
		public static void Define<T>(string name) where T : ImNode {
			Defined[name] = typeof(T);
		}

		public static ImNode Create(string name) {
			if (!Defined.TryGetValue(name, out var type)) {
				return null;
			}
			
			var node = (ImNode) Activator.CreateInstance(type);
			node.Tip = name;
			
			return node;
		}

		public static string GetName(ImNode node) {
			return node.Tip;
		}
	}
}