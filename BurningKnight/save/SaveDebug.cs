using BurningKnight.ui.imgui;
using ImGuiNET;

namespace BurningKnight.save {
	public static class SaveDebug {
		private static unsafe ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));
		private static System.Numerics.Vector2 size = new System.Numerics.Vector2(300, 400);
		private static bool global;
		
		public static void RenderDebug() {
			if (!WindowManager.Save) {
				return;
			}

			ImGui.SetNextWindowSize(size, ImGuiCond.Once);

			if (!ImGui.Begin("Save Debug")) {
				ImGui.End();
				return;
			}

			filter.Draw("Search##sd");
			ImGui.Checkbox("Global", ref global);
			ImGui.Separator();
			
			var height = ImGui.GetStyle().ItemSpacing.Y;
			ImGui.BeginChild("ScrollingRegionItems", new System.Numerics.Vector2(0, -height), 
				false, ImGuiWindowFlags.HorizontalScrollbar);

			var k = (global ? GlobalSave.Values : GameSave.Values).Keys;
			
			foreach (var i in k) {
				ImGui.PushID(i);

				if (filter.PassFilter(i)) {
					var s = i;
					var changed = false;
					
					ImGui.PushItemWidth(100);
					if (ImGui.InputText($"##{i}", ref s, 128)) {
						changed = true;
					}
					
					ImGui.PopItemWidth();
					
					var v = (global ? GlobalSave.Values : GameSave.Values)[i];

					if (v == null) {
						continue;
					}

					ImGui.SameLine();

					if (ImGui.InputText($"##v_v{i}", ref v, 128) || changed) {
						if (global) {
							GlobalSave.Values.Remove(i);
							GlobalSave.Put(s, v);
						} else {
							GameSave.Values.Remove(i);
							GameSave.Put(s, v);
						}
					}
				}

				ImGui.PopID();
			}

			ImGui.EndChild();
			ImGui.End();
		}
	}
}