using System.Collections.Generic;
using BurningKnight.ui.dialog;
using ImGuiNET;
using Lens.assets;
using Lens.lightJson;

namespace BurningKnight.ui.imgui.node {
	public class ImChoiceNode : ImNode, DialogNode {
		private List<string> choices = new List<string>();
		private string label = "";
		private string name = "";

		public ImChoiceNode() {
			AddInput();
		}

		public override string GetName() {
			return label;
		}

		public override void RenderElements() {
			if (choices.Count == 0) {
				choices.Add("");
				AddOutput();
			}
			
			Inputs[0].Offset.Y = ImGui.GetCursorPos().Y + InputHalfHeight;

			ImGui.PushItemWidth(200);
			
			if (ImGui.InputText($"##{name}", ref label, 256, ImGuiInputTextFlags.EnterReturnsTrue)) {
				name = label;
			}

			ImGui.SameLine();
				
			var add = ImGui.Button("+##choice");
			ImGui.PopItemWidth();
			ImGui.Separator();
			
			var i = 0;
			var toRemove = -1;
			
			foreach (var output in Outputs) {
				var s = choices[i];
				output.Offset.Y = ImGui.GetCursorPos().Y + InputHalfHeight;

				ImGui.Bullet();
				ImGui.SameLine();
				ImGui.PushItemWidth(200);

				ImGui.InputText($"##s_{i}", ref s, 512);
				choices[i] = s;

				ImGui.PopItemWidth();

				if (i > 0) {
					ImGui.SameLine();

					if (ImGui.Button($"-##{i}")) {
						toRemove = i;
					}
				}

				i++;
			}

			if (toRemove > -1) {
				choices.RemoveAt(toRemove);
				RemoveConnection(Outputs[toRemove], true);
			}

			if (add) {
				choices.Add("");
				AddOutput();
			}
		}

		public override void Save(JsonObject root) {
			base.Save(root);

			var i = 0;
			
			foreach (var c in choices) {
				Locale.Map[$"{LocaleId}_{i}"] = c;
				i++;
			}

			root["cc"] = choices.Count;
			Locale.Map[LocaleId] = name;
		}

		public override void Load(JsonObject root) {
			base.Load(root);

			for (var i = 0; i < root["cc"]; i++) {
				choices.Add(Locale.Map[$"{LocaleId}_{i}"]);
				AddOutput();
			}

			name = Locale.Map[LocaleId];
			label = name;
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
			
			return new Dialog(name, variants);
		}
	}
}