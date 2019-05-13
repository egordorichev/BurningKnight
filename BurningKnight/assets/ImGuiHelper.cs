using BurningKnight.ui.imgui;
using ImGuiNET;
using Lens;

namespace BurningKnight.assets {
	public static class ImGuiHelper {
		public static ImGuiRenderer Renderer;

		public static void Init() {
			Renderer = new ImGuiRenderer(Engine.Instance);
			Renderer.RebuildFontAtlas();
		}

		public static void Begin() {
			Renderer.BeforeLayout(Engine.GameTime);
		}

		public static void End() {
			Renderer.AfterLayout();
		}
	}
}