using ImGuiNET;
using Lens.assets;
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
			
			Locale.Map[LocaleId] = header;
			Locale.Map[$"{LocaleId}_a"] = optionA;
			Locale.Map[$"{LocaleId}_aa"] = answerA;
			Locale.Map[$"{LocaleId}_b"] = optionB;
			Locale.Map[$"{LocaleId}_ab"] = answerB;
		}

		public override void Load(JsonObject root) {
			base.Load(root);
			
			header = Locale.Map[LocaleId];
			optionA = Locale.Map[$"{LocaleId}_a"];
			answerA = Locale.Map[$"{LocaleId}_aa"];
			optionB = Locale.Map[$"{LocaleId}_b"];
			answerB = Locale.Map[$"{LocaleId}_ab"];
		}

		public override string GetName() {
			return header;
		}
	}
}