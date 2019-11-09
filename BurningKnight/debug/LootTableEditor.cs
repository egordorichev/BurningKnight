using BurningKnight.assets.loot;
using BurningKnight.entity.creature.drop;
using BurningKnight.ui.imgui;
using ImGuiNET;
using Lens.input;
using Microsoft.Xna.Framework.Input;

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
			
			filter.Draw("");
			ImGui.SameLine();
			ImGui.Text($"{count}");

			if (ImGui.Button("Add##pe")) {
				ImGui.OpenPopup("Add Item##pe");	
			}

			if (ImGui.BeginPopupModal("Add Item##pe")) {
				ImGui.PushItemWidth(300);
				ImGui.InputText("Id", ref poolName, 64);
				ImGui.PopItemWidth();
				
				if (ImGui.Button("Add") || Input.Keyboard.WasPressed(Keys.Enter, true)) {
					selectedTable = poolName;
					LootTables.Defined[poolName] = new AnyDrop();
					
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

			if (!ImGui.Begin("Loot Table", ref show)) {
				ImGui.End();
				return;
			}

			if (!show) {
				selectedTable = null;
				ImGui.End();
				return;
			}
			
			ImGui.Text(selectedTable);
			
			ImGui.End();
		}
	}
}