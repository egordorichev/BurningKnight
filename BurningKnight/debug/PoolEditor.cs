using BurningKnight.assets.items;
using BurningKnight.entity.item;
using BurningKnight.state;
using BurningKnight.ui.imgui;
using BurningKnight.util;
using ImGuiNET;
using Lens.input;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.debug {
	public static class PoolEditor {
		private static unsafe ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private static unsafe ImGuiTextFilterPtr popupFilter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private static System.Numerics.Vector2 size = new System.Numerics.Vector2(300, 400);
		private static System.Numerics.Vector2 popupSize = new System.Numerics.Vector2(400, 400);
		private static string selectedItem;
		private static int id;
		private static int count;
		
		public static int Pool;

		public static void Render() {
			if (!WindowManager.PoolEditor) {
				return;
			}
			
			ImGui.SetNextWindowSize(size, ImGuiCond.Once);

			if (!ImGui.Begin("Pool Editor##re")) {
				ImGui.End();
				return;
			}

			ImGui.Combo("Pool##pe", ref Pool, ItemPool.Names, ItemPool.Count);
			
			ImGui.Separator();

			filter.Draw("");
			ImGui.SameLine();
			ImGui.Text($"{count}");

			if (ImGui.Button("Add##pe")) {
				ImGui.OpenPopup("Add Item##pe");	
			}

			if (ImGui.BeginPopupModal("Add Item##pe")) {
				ImGui.SetWindowSize(popupSize);
				
				popupFilter.Draw("");
				ImGui.BeginChild("ScrollinegionUses##reee", new System.Numerics.Vector2(0, -ImGui.GetStyle().ItemSpacing.Y - ImGui.GetFrameHeightWithSpacing() - 4), 
					false, ImGuiWindowFlags.HorizontalScrollbar);
				
				ImGui.Separator();

				foreach (var i in Items.Datas) {
					ImGui.PushID($"{id}__itm");
				
					if (!BitHelper.IsBitSet(i.Value.Pools, Pool) && popupFilter.PassFilter(i.Key) && ImGui.Selectable($"{i.Key}##d", selectedItem == i.Key)) {
						selectedItem = i.Key;
					}

					ImGui.PopID();
					id++;
				}

				id = 0;

				ImGui.EndChild();
				ImGui.Separator();

				if (selectedItem != null && (ImGui.Button("Add") || Input.Keyboard.WasPressed(Keys.Enter, true))) {
					ItemEditor.Selected = Items.Datas[selectedItem];
					ItemEditor.ForceFocus = true;
					
					ItemEditor.Selected.Pools = BitHelper.SetBit(ItemEditor.Selected.Pools, Pool, true);
					ImGui.CloseCurrentPopup();
				}

				ImGui.SameLine();
				
				if (ImGui.Button("Cancel") || Input.Keyboard.WasPressed(Keys.Escape, true)) {
					ImGui.CloseCurrentPopup();
				}

				ImGui.EndPopup();
			}
			
			if (ItemEditor.Selected != null) {
				ImGui.SameLine();

				if (ImGui.Button("Remove##pe")) {
					ItemEditor.Selected.Pools = BitHelper.SetBit(ItemEditor.Selected.Pools, Pool, false);
					ItemEditor.Selected = null;
				}
			}

			count = 0;
			
			ImGui.Separator();
			
			var height = ImGui.GetStyle().ItemSpacing.Y;
			ImGui.BeginChild("rollingRegionItems##Pe", new System.Numerics.Vector2(0, -height), 
				false, ImGuiWindowFlags.HorizontalScrollbar);

			foreach (var i in Items.Datas.Values) {
				ImGui.PushID($"{id}___m");

				if (filter.PassFilter(i.Id)) {
					if (!BitHelper.IsBitSet(i.Pools, Pool)) {
						continue;
					}
					
					count++;

					if (ImGui.Selectable($"{i.Id}##ped", i == ItemEditor.Selected)) {
						if (i != ItemEditor.Selected) {
							ItemEditor.Selected = i;
							ItemEditor.ForceFocus = true;
							WindowManager.ItemEditor = true;
						}
					}
				}

				ImGui.PopID();
				id++;
			}

			id = 0;

			ImGui.EndChild();
			ImGui.End();
		}
	}
}