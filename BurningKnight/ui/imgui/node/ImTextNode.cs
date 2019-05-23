using System;
using BurningKnight.ui.dialog;
using ImGuiNET;
using Lens.assets;
using Lens.lightJson;

namespace BurningKnight.ui.imgui.node {
	public class ImTextNode : ImNode, DialogNode {
		private string name = "";
		
		public override void RenderElements() {
			if (Inputs.Count > 0) {
				Inputs[0].Offset.Y = ImGui.GetCursorPos().Y + InputHalfHeight;
			}

			if (Outputs.Count > 0) {
				Outputs[0].Offset.Y = ImGui.GetCursorPos().Y + InputHalfHeight;
			}

			ImGui.PushItemWidth(200);
			ImGui.InputText($"##{name}", ref name, 256);
			ImGui.PopItemWidth();
		}

		public override void Load(JsonObject root) {
			base.Load(root);
			name = Locale.Map[LocaleId];
		}

		public override void Save(JsonObject root) {
			base.Save(root);
			Locale.Map[LocaleId] = name;
		}

		public override string GetName() {
			return name;
		}

		public Dialog Convert() {
			if (Outputs.Count < 2) {
				return new Dialog(LocaleId, Outputs.Count == 0 ? null : Outputs[0].Parent.LocaleId);
			}

			throw new Exception("Dialog can not have more than 1 output");
		}
	}
}