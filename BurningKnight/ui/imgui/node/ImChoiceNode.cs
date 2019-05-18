using System.Collections.Generic;
using ImGuiNET;
using Lens.lightJson;

namespace BurningKnight.ui.imgui.node {
	public class ImChoiceNode : ImNode {
		private List<string> choices = new List<string>();
		private string label = "";
		private string name = "";

		public ImChoiceNode() {
			AddInput();
		}

		public override string GetName() {
			return label;
		}

		public override void RenderElements() {
			if (choices.Count == 0) {
				choices.Add("");
				AddOutput();
			}
			
			Inputs[0].Offset.Y = ImGui.GetCursorPos().Y + InputHalfHeight;

			ImGui.PushItemWidth(200);
			
			if (ImGui.InputText($"##{name}", ref label, 256, ImGuiInputTextFlags.EnterReturnsTrue)) {
				name = label;
			}

			ImGui.SameLine();
				
			var add = ImGui.Button("+##choice");
			ImGui.PopItemWidth();
			ImGui.Separator();
			
			var i = 0;
			var toRemove = -1;
			
			foreach (var output in Outputs) {
				var s = choices[i];
				output.Offset.Y = ImGui.GetCursorPos().Y + InputHalfHeight;

				ImGui.Bullet();
				ImGui.SameLine();
				ImGui.PushItemWidth(200);

				ImGui.InputText($"##s_{i}", ref s, 512);
				choices[i] = s;

				ImGui.PopItemWidth();

				if (i > 0) {
					ImGui.SameLine();

					if (ImGui.Button($"-##{i}")) {
						toRemove = i;
					}
				}

				i++;
			}

			if (toRemove > -1) {
				choices.RemoveAt(toRemove);
				RemoveConnection(Outputs[toRemove], true);
			}

			if (add) {
				choices.Add("");
				AddOutput();
			}
		}

		public override void Save(JsonObject root) {
			base.Save(root);

			var ch = new JsonArray();

			foreach (var c in choices) {
				ch.Add(c);
			}

			root["choices"] = ch;
			root["name"] = name;
		}

		public override void Load(JsonObject root) {
			base.Load(root);

			var ch = root["choices"].AsJsonArray;

			foreach (var c in ch) {
				choices.Add(c);
				AddOutput();
			}

			name = root["name"];
			label = name;
		}
	}
}