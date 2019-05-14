using System.Collections.Generic;
using BurningKnight.ui.imgui;
using BurningKnight.ui.imgui.node;
using ImGuiNET;
using Lens;

namespace BurningKnight.assets {
	public static class ImGuiHelper {
		public static ImGuiRenderer Renderer;
		public static Dictionary<int, ImNode> Nodes = new Dictionary<int, ImNode>();
		
		public static void Init() {
			Renderer = new ImGuiRenderer(Engine.Instance);
		}

		private static bool loadedFont;

		public static void Begin() {
			if (!loadedFont) {
				loadedFont = true;
				Renderer.RebuildFontAtlas();
			}
			
			Renderer.BeforeLayout(Engine.GameTime);
		}

		public static void End() {
			Renderer.AfterLayout();
		}

		public static void ClearNodes() {
			Nodes.Clear();
		}

		public static void Node(ImNode node) {
			Nodes[node.Id] = node;
		}

		public static void RenderNodes() {
			foreach (var n in Nodes) {
				n.Value.Render();
			}

			if (!ImGui.Begin("Nodes", ImGuiWindowFlags.NoBringToFrontOnFocus | ImGuiWindowFlags.NoFocusOnAppearing)) {
				ImGui.End();
				return;
			}
			
			foreach (var p in Nodes) {
				var node = p.Value;
				
				if (ImGui.Selectable($"{node.GetName()} #{node.Id}", ImNode.Focused == node)) {
					ImNode.Focused = node;
					node.ForceFocus = true;
				}
			}
			
			ImGui.End();
		}

		public static ImNode CurrentActive;
	}
}