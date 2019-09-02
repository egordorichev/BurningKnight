using ImGuiNET;
using Lens.lightJson;
using Lens.util;
using Lens.util.tween;

namespace BurningKnight.entity.item.renderer {
	public class MovingAngledRenderer : AngledRenderer {
		public float MaxAngle;		
		public float MinAngle;
		public bool Stay;
		public float SwingTime;
		public float ReturnTime;
		private bool stayed;
		
		public override void OnUse() {
			var task = Tween.To(stayed ? MinAngle : MaxAngle, SwingAngle, x => SwingAngle = x, SwingTime);
			
			if (!Stay) {
				stayed = !stayed;

				task.OnEnd = () => {
					Tween.To(stayed ? MinAngle : MaxAngle, SwingAngle, x => SwingAngle = x, ReturnTime);
				};
			}
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			Stay = settings["stay"].Bool(false);
			MaxAngle = settings["max_angle"].Number(180).ToRadians();
			MinAngle = settings["min_angle"].Number(0).ToRadians();
			SwingTime = settings["st"].Number(0.1f);
			ReturnTime = settings["rt"].Number(0.2f);

			SwingAngle = MinAngle;
		}

		public static void RenderDebug(string id, JsonValue parent, JsonValue root) {
			AngledRenderer.RenderDebug(id, parent, root);

			var min = (float) root["min_angle"].Number(0);

			if (ImGui.InputFloat("Min Angle", ref min)) {
				root["min_angle"] = min;
			}
			
			var max = (float) root["max_angle"].Number(180);

			if (ImGui.InputFloat("Max Angle", ref max)) {
				root["max_angle"] = max;
			}
			
			var stay = root["stay"].AsBoolean;

			if (ImGui.Checkbox("Stay?", ref stay)) {
				root["stay"] = stay;
			}

			var st = (float) root["st"].Number(0.1f);

			if (ImGui.InputFloat("Swing Time", ref st)) {
				root["st"] = st;
			}

			if (!stay) {
				var rt = (float) root["rt"].Number(0.2f);

				if (ImGui.InputFloat("Return Time", ref rt)) {
					root["rt"] = rt;
				}
			}
		}
	}
}