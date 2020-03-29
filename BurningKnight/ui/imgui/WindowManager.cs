using System.Collections.Generic;
using System.Numerics;
using BurningKnight.assets.lighting;
using BurningKnight.debug;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.room;
using BurningKnight.level.rooms.regular;
using BurningKnight.save;
using BurningKnight.save.statistics;
using BurningKnight.state;
using ImGuiNET;
using Lens;
using Lens.assets;
using Lens.entity;

namespace BurningKnight.ui.imgui {
	public static class WindowManager {
		private static Vector2 size;

		public static bool Cheats;
		public static bool Debug;
		public static bool Entities;
		public static bool RunInfo;
		public static bool Console = true;
		public static bool ItemEditor;
		public static bool LevelEditor;
		public static bool LayerDebug;
		public static bool LocaleEditor;
		public static bool Rooms;
		public static bool Settings;
		public static bool Lighting;
		public static bool Achievements;
		public static bool Save;
		public static bool PoolEditor;
		public static bool LootTable;
		
		private static void RenderSettings() {
			if (!Settings) {
				return;
			}
			
			if (!ImGui.Begin("Settings")) {
				ImGui.End();
				return;
			}
			
			var v = AudioEmitterComponent.PositionScale * 100;
			ImGui.InputFloat("Position scale", ref v);
			AudioEmitterComponent.PositionScale = v / 100;
			ImGui.InputFloat("Distance", ref AudioEmitterComponent.Distance);
			ImGui.InputFloat("Speed", ref Audio.Speed);

			ImGui.End();
		}
		
		public static void Render(Area Area) {
			CheatWindow.Render();
			AreaDebug.Render(Area);
			DebugWindow.Render();
			
			imgui.LocaleEditor.Render();			
			debug.PoolEditor.Render();
			state.ItemEditor.Render();
			RenderSettings();
			Lights.RenderDebug();
			SaveDebug.RenderDebug();
			LevelLayerDebug.Render();
			LootTableEditor.Render();
			assets.achievements.Achievements.RenderDebug();

			RunStatistics.RenderDebug();

			if (Rooms && ImGui.Begin("Rooms", ImGuiWindowFlags.AlwaysAutoResize)) {
				var p = LocalPlayer.Locate(Area);

				if (p != null) {
					var rm = p.GetComponent<RoomComponent>().Room;
					var rn = new List<Room>();

					foreach (var r in Area.Tagged[Tags.Room]) {
						rn.Add((Room) r);
					}
					
					rn.Sort((a, b) => a.Type.CompareTo(b.Type));
					
					foreach (var r in rn) {
						var v = rm == r;

						if (ImGui.Selectable($"{r.Type}#{r.Y}", ref v) && v) {
							p.Center = r.Center;
						}
					}
				}

				ImGui.End();
			}
			
			ImGui.SetNextWindowPos(new Vector2(Engine.Instance.GetScreenWidth() - size.X - 10, Engine.Instance.GetScreenHeight() - size.Y - 10));
			ImGui.Begin("Windows", ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoTitleBar);

			if (ImGui.Button("Hide")) {
				Debug = Entities = RunInfo = Console = ItemEditor = LevelEditor = LocaleEditor = Rooms = Settings = PoolEditor = LootTable = false;
			}
			
			ImGui.SameLine();

			if (ImGui.Button("Show")) {
				Debug = Entities = RunInfo = Console = ItemEditor = LevelEditor = LocaleEditor = Rooms = Settings = PoolEditor = LootTable = true;
			}
			
			ImGui.Separator();
			
			ImGui.Checkbox("Cheats", ref Cheats);
			ImGui.Checkbox("Entities", ref Entities);
			ImGui.Checkbox("Run Info", ref RunInfo);
			ImGui.Checkbox("Console", ref Console);
			ImGui.Checkbox("Item Editor", ref ItemEditor);
			ImGui.Checkbox("Level Editor", ref LevelEditor);
			ImGui.Checkbox("Layer Debug", ref LayerDebug);
			ImGui.Checkbox("Locale Editor", ref LocaleEditor);
			ImGui.Checkbox("Debug", ref Debug);
			ImGui.Checkbox("Rooms", ref Rooms);
			ImGui.Checkbox("Settings", ref Settings);
			ImGui.Checkbox("Lighting", ref Lighting);
			ImGui.Checkbox("Achievements", ref Achievements);
			ImGui.Checkbox("Save", ref Save);
			ImGui.Checkbox("Pool Editor", ref PoolEditor);
			ImGui.Checkbox("Loot Table Editor", ref LootTable);
			
			size = ImGui.GetWindowSize();
			ImGui.End();
		}
	}
}