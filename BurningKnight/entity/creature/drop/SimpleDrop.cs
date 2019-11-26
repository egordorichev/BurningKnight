using System.Collections.Generic;
using BurningKnight.state;
using BurningKnight.ui.imgui;
using BurningKnight.util;
using ImGuiNET;
using Lens.input;
using Lens.lightJson;
using Lens.util.math;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.entity.creature.drop {
	public class SimpleDrop : Drop {
		private static System.Numerics.Vector2 popupSize = new System.Numerics.Vector2(400, 400);
		private static unsafe ImGuiTextFilterPtr popupFilter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private static string selectedItem;
		private static int id;
		
		public string[] Items;
		public int Min = 1;
		public int Max = 1;

		public SimpleDrop(float chance, int min, int max, params string[] items) {
			Chance = chance;
			Min = min;
			Max = max;
			Items = items;
		}
		
		public SimpleDrop() {
			
		}
		
		public override List<string> GetItems() {
			var items = new List<string>();

			if (Items != null) {
				for (var i = 0; i < Rnd.Int(Min, Max + 1); i++) {
					foreach (var item in Items) {
						if (assets.items.Items.ShouldAppear(item)) {
							items.Add(item);
						}
					}
				}
			}
			
			return items;
		}

		public override string GetId() {
			return "simple";
		}

		public override void Load(JsonValue root) {
			base.Load(root);
			
			Min = root["min"].Int(1);
			Max = root["max"].Int(1);

			if (root["items"].IsJsonArray) {
				var items = root["items"].AsJsonArray;
				Items = new string[items.Count];

				for (var i = 0; i < Items.Length; i++) {
					Items[i] = items[i].AsString;
				}
			}
		}

		public override void Save(JsonValue root) {
			base.Save(root);

			var items = new JsonArray();

			foreach (var item in Items) {
				items.Add(item);
			}
			
			root["min"] = Min;
			root["max"] = Max;
			root["items"] = items;
		}

		public static void RenderDebug(JsonValue root) {
			root.InputFloat("Chance", "chance");
			
			root.InputInt("Min", "min");
			root.InputInt("Max", "max");
			
			if (!root["items"].IsJsonArray) {
				root["items"] = new JsonArray();
			}

			var toRemove = -1;
			var items = root["items"].AsJsonArray;

			for (var i = 0; i < items.Count; i++) {
				if (ImGui.SmallButton($"{items[i]}##s")) {
					WindowManager.ItemEditor = true;
					ItemEditor.Selected = assets.items.Items.Datas[items[i]];
				}
				
				ImGui.SameLine();

				if (ImGui.SmallButton("-")) {
					toRemove = i;
				}
			}

			if (toRemove != -1) {
				items.Remove(toRemove);
			}

			if (ImGui.Button("Add")) {
				ImGui.OpenPopup("Add Item##p");	
			}
			
			ImGui.Separator();

			if (ImGui.BeginPopupModal("Add Item##p")) {
				ImGui.SetWindowSize(popupSize);
				
				popupFilter.Draw("");
				ImGui.BeginChild("ScrollinegionUses##reee", new System.Numerics.Vector2(0, -ImGui.GetStyle().ItemSpacing.Y - ImGui.GetFrameHeightWithSpacing() - 4), 
					false, ImGuiWindowFlags.HorizontalScrollbar);
				
				ImGui.Separator();

				foreach (var i in assets.items.Items.Datas) {
					ImGui.PushID($"{id}__itm");
				
					if (popupFilter.PassFilter(i.Key) && !items.Contains(i.Key) && ImGui.Selectable($"{i.Key}##dd", selectedItem == i.Key)) {
						selectedItem = i.Key;
					}

					ImGui.PopID();
					id++;
				}

				id = 0;

				ImGui.EndChild();
				ImGui.Separator();

				if (selectedItem != null && (ImGui.Button("Add") || Input.Keyboard.WasPressed(Keys.Enter, true))) {
					items.Add(selectedItem);
					selectedItem = null;
					ImGui.CloseCurrentPopup();
				}

				ImGui.SameLine();
				
				if (ImGui.Button("Cancel") || Input.Keyboard.WasPressed(Keys.Escape, true)) {
					ImGui.CloseCurrentPopup();
				}

				ImGui.EndPopup();
			}
		}
	}
}