using BurningKnight.ui.imgui;
using ImGuiNET;

namespace BurningKnight.debug {
	public static class CheatWindow {
		public static bool AutoGodMode = BK.Version.Dev;
		public static bool InfiniteActive;
		
		public static void Render() {
			if (!WindowManager.Cheats) {
				return;
			}

			if (!ImGui.Begin("Cheats", ImGuiWindowFlags.AlwaysAutoResize)) {
				ImGui.End();
				return;
			}

			ImGui.Checkbox("Auto god mode", ref AutoGodMode);
			ImGui.Checkbox("Infinite active charge", ref InfiniteActive);
			
			ImGui.End();
		}
	}
}