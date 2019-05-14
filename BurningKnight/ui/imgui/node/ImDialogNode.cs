using ImGuiNET;
using Lens.lightJson;

namespace BurningKnight.ui.imgui.node {
	public class ImDialogNode : ImNode {
		private const int inputHalfHeight = 8;

		private string header = "Alarm.";
		private string optionA = "Wake.";
		private string optionB = "Snore.";
		private string answerA = "Early.";
		private string answerB = "Yeee.";

		public ImDialogNode() {
			
		}
		
		public ImDialogNode(string name) : base(name) {
			AddInput();
			
			AddOutput();
			AddOutput();
		}

		public override void RenderElements() {
			ImGui.PushItemWidth(200);
			Inputs[0].Offset.Y = ImGui.GetCursorPos().Y + inputHalfHeight;
			ImGui.InputText($"Header##{Id}", ref header, 128);
			
			ImGui.Separator();
			ImGui.InputText($"Option A##{Id}", ref optionA, 128);
			Outputs[0].Offset.Y = ImGui.GetCursorPos().Y + inputHalfHeight;
			ImGui.InputText($"Answer A##{Id}", ref answerA, 128);
			
			ImGui.Separator();
			ImGui.InputText($"Option B##{Id}", ref optionB, 128);
			Outputs[1].Offset.Y = ImGui.GetCursorPos().Y + inputHalfHeight;
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
	}
}