using ImGuiNET;
using Lens.lightJson;

namespace BurningKnight.util {
	public static class ImJson {
		public static bool Checkbox(this JsonValue value, string name, string id, bool d = true) {
			var val = value[id].Bool(d);
			ImGui.Checkbox(name, ref val);
			value[id] = val;

			return val;
		}
		
		public static int InputInt(this JsonValue value, string name, string id, int d = 1) {
			var val = value[id].Int(d);
			ImGui.InputInt(name, ref val);
			value[id] = val;

			return val;
		}
		
		public static float InputFloat(this JsonValue value, string name, string id, float d = 1) {
			var val = value[id].Number(d);
			ImGui.InputFloat(name, ref val);
			value[id] = val;

			return val;
		}
		
		public static string InputText(this JsonValue value, string name, string id, string d = "", uint length = 64) {
			var val = value[id].String(d) ?? "";

			ImGui.InputText(name, ref val, length); 
			value[id] = val;

			return val;
		}
	}
}