using BurningKnight.assets.loot;
using BurningKnight.entity.creature.drop;
using BurningKnight.ui.imgui;
using ImGuiNET;
using Lens.input;
using Lens.lightJson;
using Microsoft.Xna.Framework.Input;
using ImGui = ImGuiNET.ImGui;

namespace BurningKnight.debug {
	public static class LootTableEditor {
		private static unsafe ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private static System.Numerics.Vector2 size = new System.Numerics.Vector2(300, 400);

		private static int id;
		private static int count;
		private static string selectedTable;
		private static string poolName = "";
	
		public static void Render() {
			if (!WindowManager.LootTable) {
				return;
			}
			
			ImGui.SetNextWindowSize(size, ImGuiCond.Once);

			if (!ImGui.Begin("Loot Table Editor")) {
				ImGui.End();
				return;
			}

			if (ImGui.Button("Save")) {
				LootTables.Save();
			}

			ImGui.SameLine();
			
			if (ImGui.Button("New##pe")) {
				ImGui.OpenPopup("Add Item##pe");	
			}

			if (selectedTable != null) {
				ImGui.SameLine();
				
				if (ImGui.Button("Delete")) {
					LootTables.Defined.Remove(selectedTable);
					LootTables.Data.Remove(selectedTable);
					
					selectedTable = null;
				}
			}
			
			filter.Draw("");
			ImGui.SameLine();
			ImGui.Text($"{count}");

			if (ImGui.BeginPopupModal("Add Item##pe")) {
				ImGui.PushItemWidth(300);
				ImGui.InputText("Id", ref poolName, 64);
				ImGui.PopItemWidth();
				
				if (ImGui.Button("Add") || Input.Keyboard.WasPressed(Keys.Enter, true)) {
					selectedTable = poolName;
					LootTables.Defined[poolName] = new AnyDrop();
					LootTables.Data[poolName] = new JsonObject {
						["type"] = "any"
					};
					
					poolName = "";
					ImGui.CloseCurrentPopup();
				}

				ImGui.SameLine();
				
				if (ImGui.Button("Cancel") || Input.Keyboard.WasPressed(Keys.Escape, true)) {
					poolName = "";
					ImGui.CloseCurrentPopup();
				}

				ImGui.EndPopup();
			}
			
			if (selectedTable != null) {
				ImGui.SameLine();

				if (ImGui.Button("Remove##pe")) {
					LootTables.Defined.Remove(selectedTable);
					selectedTable = null;
				}
			}

			count = 0;
			
			ImGui.Separator();
			
			var height = ImGui.GetStyle().ItemSpacing.Y;
			ImGui.BeginChild("rolingRegionItems##Pe", new System.Numerics.Vector2(0, -height), 
				false, ImGuiWindowFlags.HorizontalScrollbar);

			foreach (var i in LootTables.Defined) {
				ImGui.PushID($"{id}___m");

				if (filter.PassFilter(i.Key)) {
					count++;

					if (ImGui.Selectable($"{i.Key}##ped", i.Key  == selectedTable)) {
						selectedTable = i.Key;
					}
				}

				ImGui.PopID();
				id++;
			}

			id = 0;

			ImGui.EndChild();
			ImGui.End();

			if (selectedTable == null) {
				return;
			}

			var show = true;
			ImGui.SetNextWindowSize(size, ImGuiCond.Once);

			if (!ImGui.Begin("Loot Table", ref show)) {
				ImGui.End();
				return;
			}

			if (!show) {
				selectedTable = null;
				ImGui.End();
				return;
			}
			
			LootTables.RenderDrop(LootTables.Data[selectedTable]);
			ImGui.End();
		}
	}
}