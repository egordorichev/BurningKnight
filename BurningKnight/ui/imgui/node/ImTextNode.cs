using ImGuiNET;
using Lens.lightJson;

namespace BurningKnight.ui.imgui.node {
	public class ImTextNode : ImNode {
		private const int inputHalfHeight = 8;
		private string name = "";
		private string label = "";
		
		public override void RenderElements() {
			if (Inputs.Count > 0) {
				Inputs[0].Offset.Y = ImGui.GetCursorPos().Y + inputHalfHeight;
			}

			if (Outputs.Count > 0) {
				Outputs[0].Offset.Y = ImGui.GetCursorPos().Y + inputHalfHeight;
			}
			
			ImGui.PushItemWidth(200);
			
			if (ImGui.InputText($"##{name}", ref label, 256, ImGuiInputTextFlags.EnterReturnsTrue)) {
				name = label;
			}
			
			ImGui.PopItemWidth();
		}

		public override void Load(JsonObject root) {
			base.Load(root);
			name = root["name"];
		}

		public override void Save(JsonObject root) {
			base.Save(root);
			root["name"] = name;
		}

		public override string GetName() {
			return name;
		}
	}
}