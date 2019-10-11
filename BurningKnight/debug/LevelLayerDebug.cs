using BurningKnight.ui.imgui;
using ImGuiNET;

namespace BurningKnight.debug {
	public static class LevelLayerDebug {
		public static bool Chasms = true;
		public static bool Floor = true;
		public static bool Liquids = true;
		public static bool Mess = true;
		public static bool Sides = true;
		public static bool Walls = true;
		public static bool Blood = true;
		public static bool Lights = true;
		public static bool TileLight = true;
		public static bool Shadows = true;
				
		public static void Render() {
			if (!WindowManager.LayerDebug) {
				return;
			}

			if (!ImGui.Begin("Layer Debug", ImGuiWindowFlags.AlwaysAutoResize)) {
				ImGui.End();
				return;
			}

			if (ImGui.Button("Show")) {
				Chasms = Floor = Liquids = Mess = Sides = Walls = Blood = Lights = TileLight = Shadows = true;
			}
			
			ImGui.SameLine();

			if (ImGui.Button("Hide")) {
				Chasms = Floor = Liquids = Mess = Sides = Walls = Blood = Lights = TileLight = Shadows = false;
			}

			ImGui.Checkbox("Chasms", ref Chasms);
			ImGui.Checkbox("Floor", ref Floor);
			ImGui.Checkbox("Mess", ref Mess);
			ImGui.Checkbox("Liquids", ref Liquids);
			ImGui.Checkbox("Shadows", ref Shadows);
			ImGui.Checkbox("Sides", ref Sides);
			ImGui.Checkbox("Walls", ref Walls);
			ImGui.Checkbox("Blood", ref Blood);
			ImGui.Checkbox("Lights", ref Lights);
			ImGui.Checkbox("Tile Light", ref TileLight);
			
			ImGui.End();
		}
	}
}