using BurningKnight.ui.dialog;
using ImGuiNET;
using Lens.lightJson;

namespace BurningKnight.ui.imgui.node {
	public class ImAnswerNode : ImDialogNode {
		private int type;
		
		public override void RenderElements() {
			base.RenderElements();
			ImGui.Combo("Type", ref type, AnswerDialog.Types, AnswerDialog.Types.Length);
		}

		protected override Dialog CreateDialog(string id, string[] variants) {
			return new AnswerDialog(id, (AnswerType) type, variants);
		}

		public override void Save(JsonObject root) {
			base.Save(root);
			root["atype"] = type;
		}

		public override void Load(JsonObject root) {
			base.Load(root);
			type = root["atype"].Int(0);
		}
	}
}