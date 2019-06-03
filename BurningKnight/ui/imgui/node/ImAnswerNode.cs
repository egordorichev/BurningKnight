using BurningKnight.ui.dialog;
using ImGuiNET;

namespace BurningKnight.ui.imgui.node {
	public class ImAnswerNode : ImDialogNode {
		public int type;
		
		public override void RenderElements() {
			base.RenderElements();
			ImGui.Combo("Type", ref type, AnswerDialog.Types, AnswerDialog.Types.Length);
		}

		protected override Dialog CreateDialog(string id, string[] variants) {
			return new AnswerDialog(id, (AnswerType) type, variants);
		}
	}
}