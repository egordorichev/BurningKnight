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
			string[] variants = null;

			if (Outputs.Count > 0) {
				variants = new string[Outputs.Count];
				var i = 0;
				
				foreach (var o in Outputs) {
					variants[i] = o.Parent.LocaleId;
					i++;
				}
			}
			
			return new Dialog(LocaleId, variants);
		}
	}
}