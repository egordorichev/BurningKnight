using ImGuiNET;
using Lens.lightJson;

namespace BurningKnight.util {
	public static class ImJson {
		public static bool Checkbox(this JsonValue value, string name, string id, bool d = true) {
			var val = value[id].Bool(d);

			if (ImGui.Checkbox(name, ref val)) {
				value[id] = val;
			}

			return val;
		}
		
		public static float InputFloat(this JsonValue value, string name, string id, float d = 1) {
			var val = value[id].Number(d);

			if (ImGui.InputFloat(name, ref val)) {
				value[id] = val;
			}

			return val;
		}
		
		public static string InputText(this JsonValue value, string name, string id, string d = "", uint length = 64) {
			var val = value[id].String(d);

			if (ImGui.InputText(name, ref val, length)) {
				value[id] = val;
			}

			return val;
		}
	}
}