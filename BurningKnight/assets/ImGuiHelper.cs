using System.Collections.Generic;
using BurningKnight.ui.imgui;
using Lens;

namespace BurningKnight.assets {
	public static class ImGuiHelper {
		public static ImGuiRenderer Renderer;
		public static List<ImNode> Nodes = new List<ImNode>();
		
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
			Nodes.Add(node);
		}

		public static void RenderNodes() {
			foreach (var n in Nodes) {
				n.Render();
			}
		}

		public static ImNode CurrentActive;
	}
}