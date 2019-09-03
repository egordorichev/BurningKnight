using System.Numerics;
using ImGuiNET;
using Lens;

namespace BurningKnight.ui.imgui {
	public static class WindowManager {
		private static Vector2 size;

		public static bool Debug;
		public static bool Entities;
		public static bool RunInfo;
		public static bool Console = true;
		public static bool ItemEditor;
		public static bool LevelEditor;
		public static bool LocaleEditor;
		public static bool Rooms;
		public static bool Settings;
		
		public static void Render() {
			ImGui.SetNextWindowPos(new Vector2(Engine.Instance.GetScreenWidth() - size.X - 10, Engine.Instance.GetScreenHeight() - size.Y - 10));
			ImGui.Begin("Windows", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoTitleBar);

			if (ImGui.Button("Hide")) {
				Debug = Entities = RunInfo = Console = ItemEditor = LevelEditor = LocaleEditor = Rooms = Settings = false;
			}
			
			ImGui.SameLine();

			if (ImGui.Button("Show")) {
				Debug = Entities = RunInfo = Console = ItemEditor = LevelEditor = LocaleEditor = Rooms = Settings = true;
			}
			
			ImGui.Separator();
			
			ImGui.Checkbox("Entities", ref Entities);
			ImGui.Checkbox("Run Info", ref RunInfo);
			ImGui.Checkbox("Console", ref Console);
			ImGui.Checkbox("Item Editor", ref ItemEditor);
			ImGui.Checkbox("Level Editor", ref LevelEditor);
			ImGui.Checkbox("Locale Editor", ref LocaleEditor);
			ImGui.Checkbox("Debug", ref Debug);
			ImGui.Checkbox("Rooms", ref Rooms);
			ImGui.Checkbox("Settings", ref Settings);
			
			size = ImGui.GetWindowSize();
			ImGui.End();
		}
	}
}