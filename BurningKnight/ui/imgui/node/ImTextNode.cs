using BurningKnight.ui.dialog;
using ImGuiNET;
using Lens.assets;
using Lens.lightJson;

namespace BurningKnight.ui.imgui.node {
	public class ImTextNode : ImNode, DialogNode {
		private string name = "";
		private string label = "";
		
		public override void RenderElements() {
			if (Inputs.Count > 0) {
				Inputs[0].Offset.Y = ImGui.GetCursorPos().Y + InputHalfHeight;
			}

			if (Outputs.Count > 0) {
				Outputs[0].Offset.Y = ImGui.GetCursorPos().Y + InputHalfHeight;
			}

			ImGui.PushItemWidth(200);
			
			if (ImGui.InputText($"##{name}", ref label, 256, ImGuiInputTextFlags.EnterReturnsTrue)) {
				name = label;
			}
			
			ImGui.PopItemWidth();
		}

		public override void Load(JsonObject root) {
			base.Load(root);
			name = Locale.Map[LocaleId];
			label = name;
		}

		public override void Save(JsonObject root) {
			base.Save(root);
			name = label;
			Locale.Map[LocaleId] = name;
		}

		public override string GetName() {
			return name;
		}

		public Dialog Convert() {
			string[] variants = null;

			if (Outputs.Count == 1) {
				var t = Outputs[0].ConnectedTo;
				variants = new string[t.Count];
				var i = 0;
				
				foreach (var o in t) {
					variants[i] = o.Parent.LocaleId;
					i++;
				}
			}
			
			return new Dialog(LocaleId, variants);
		}
	}
}