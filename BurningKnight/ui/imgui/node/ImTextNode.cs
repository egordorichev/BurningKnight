using ImGuiNET;

namespace BurningKnight.ui.imgui.node {
	public class ImTextNode : ImNode {
		private const int inputHalfHeight = 8;

		public ImTextNode() {
			
		}
		
		public ImTextNode(string name) : base(name) {
			
		}

		public override void RenderElements() {
			if (Inputs.Count > 0) {
				Inputs[0].Offset.Y = ImGui.GetCursorPos().Y + inputHalfHeight;
			}

			if (Outputs.Count > 0) {
				Outputs[0].Offset.Y = ImGui.GetCursorPos().Y + inputHalfHeight;
			}
			
			ImGui.Text(Name);
		}
	}
}