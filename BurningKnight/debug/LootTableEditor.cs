using BurningKnight.ui.imgui;
using ImGuiNET;

namespace BurningKnight.debug {
	public static class LootTableEditor {
		public static void Render() {
			if (!WindowManager.LootTable) {
				return;
			}

			if (!ImGui.Begin("Loot Table Editor")) {
				ImGui.End();
				return;
			}
			
			ImGui.End();
		}
	}
}