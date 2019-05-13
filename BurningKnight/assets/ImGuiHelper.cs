using BurningKnight.ui.imgui;
using ImGuiNET;
using Lens;

namespace BurningKnight.assets {
	public static class ImGuiHelper {
		public static ImGuiRenderer Renderer;

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
	}
}