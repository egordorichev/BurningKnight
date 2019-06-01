using BurningKnight.ui.dialog;
using ImGuiNET;

namespace BurningKnight.ui.imgui.node {
	public class ImAnswerNode : ImDialogNode {
		public override void RenderElements() {
			ImGui.Text("Answer node");
			base.RenderElements();
		}

		protected override Dialog CreateDialog(string id, string[] variants) {
			return new AnswerDialog(id, variants);
		}
	}
}