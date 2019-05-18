using ImGuiNET;
using Lens.lightJson;

namespace BurningKnight.ui.imgui.node {
	public class ImOrdNode : ImNode {
		private string header = "Title";
		private string optionA = "Option A";
		private string optionB = "Option B";
		private string answerA = "Answer A";
		private string answerB = "Answer B";

		public ImOrdNode() {
			AddInput();
			
			AddOutput();
			AddOutput();
		}

		public override void RenderElements() {
			ImGui.PushItemWidth(200);
			Inputs[0].Offset.Y = ImGui.GetCursorPos().Y + InputHalfHeight;
			ImGui.InputText($"Header##{Id}", ref header, 128);
			
			ImGui.Separator();
			ImGui.InputText($"Option A##{Id}", ref optionA, 128);
			Outputs[0].Offset.Y = ImGui.GetCursorPos().Y + InputHalfHeight;
			ImGui.InputText($"Answer A##{Id}", ref answerA, 128);
			
			ImGui.Separator();
			ImGui.InputText($"Option B##{Id}", ref optionB, 128);
			Outputs[1].Offset.Y = ImGui.GetCursorPos().Y + InputHalfHeight;
			ImGui.InputText($"Answer B##{Id}", ref answerB, 128);
			ImGui.PopItemWidth();
		}

		public override void Save(JsonObject root) {
			base.Save(root);
			
			root["header"] = header;
			root["optionA"] = optionA;
			root["optionB"] = optionB;
			root["answerA"] = answerA;
			root["answerB"] = answerB;
		}

		public override void Load(JsonObject root) {
			base.Load(root);
			
			header = root["header"].AsString;
			optionA = root["optionA"].AsString;
			optionB = root["optionB"].AsString;
			answerA = root["answerA"].AsString;
			answerB = root["answerB"].AsString;
		}

		public override string GetName() {
			return header;
		}
	}
}