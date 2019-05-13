using System.Collections.Generic;
using ImGuiNET;
using Lens.util.file;

namespace BurningKnight.state.save {
	public unsafe class GlobalInspector : SaveInspector {
		public Dictionary<string, string> Values = new Dictionary<string, string>();

		public override void Inspect(FileReader reader) {
			var count = reader.ReadInt32();

			for (var i = 0; i < count; i++) {
				var key = reader.ReadString();
				var val = reader.ReadString();

				Values[key] = val;
			}
		}

		private ImGuiTextFilterPtr filter = new ImGuiTextFilterPtr(ImGuiNative.ImGuiTextFilter_ImGuiTextFilter(null));

		public override void Render() {
			ImGui.Text($"Global Save ({Values.Count} entries)");
			filter.Draw();

			foreach (var pair in Values) {
				if (filter.PassFilter(pair.Key)) {
					ImGui.BulletText(pair.Key);
					ImGui.SameLine();
					ImGui.Text(pair.Value);
				}
			}
		}
	}
}