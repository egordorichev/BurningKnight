using System.Collections.Generic;
using ImGuiNET;

namespace BurningKnight.ui.imgui.node {
	public class ImChoiceNode : ImNode {
		private List<string> choices = new List<string>();

		public ImChoiceNode() {
			AddOutput();
			choices.Add("test");
			AddOutput();
			choices.Add("test2");
		}

		public override void RenderElements() {
			var i = 0;
			
			foreach (var output in Outputs) {
				var s = choices[i];

				ImGui.PushItemWidth(200);
				
				if (ImGui.InputText($"##s_{i}", ref s, 512, ImGuiInputTextFlags.EnterReturnsTrue)) {
					choices[i] = s;
				}

				ImGui.PopItemWidth();

				if (i > 0) {
					ImGui.SameLine();
					ImGui.Button("-");
				}
				
				i++;
			}
		}
	}
}