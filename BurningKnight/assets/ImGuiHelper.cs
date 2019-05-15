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

		private static List<int> toRemove = new List<int>();
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
				var node = n.Value;
				
				node.Render();

				if (node.Done) {
					toRemove.Add(n.Key);
				}
			}

			if (toRemove.Count > 0) {
				foreach (var k in toRemove) {
					Nodes.Remove(k);
				}
				
				toRemove.Clear();
			}
			
			if (!ImGui.Begin("Nodes")) {
				ImGui.End();
				return;
			}

			RenderMenu();
			
			foreach (var p in Nodes) {
				var node = p.Value;

				if (ImGui.Selectable($"{node.GetName()} #{node.Id}", ImNode.Focused == node)) {
					ImNode.Focused = node;
					node.ForceFocus = true;
				}

				if (ImGui.OpenPopupOnItemClick("node_menu", 1)) {
					CurrentMenu = node;
					
				}
			}
			
			ImGui.End();
		}

		public static ImNode CurrentMenu;
		
		public static void RenderMenu(bool window = false) {
			if (window ? ImGui.BeginPopupContextWindow("window_node_menu", 1) : ImGui.BeginPopupContextItem("node_menu", 1)) {
				
				if (ImGui.Selectable("Copy (Ctrl+C)")) {
					
				}

				if (ImGui.Selectable("Paste (Ctrl+V)")) {
					
				}
				
				if (ImGui.Selectable("Delete (Delete)")) {
					CurrentMenu.Remove();
				}
				
				ImGui.EndPopup();
			}
		}

		public static ImNode CurrentActive;
	}
}