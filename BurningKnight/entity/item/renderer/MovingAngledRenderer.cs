using ImGuiNET;
using Lens.lightJson;
using Lens.util;
using Lens.util.tween;

namespace BurningKnight.entity.item.renderer {
	public class MovingAngledRenderer : AngledRenderer {
		public float MaxAngle;
		public bool Stay;
		
		public override void OnUse() {
			var task = Tween.To(Angle > 1 ? 0 : MaxAngle, Angle, x => Angle = x, 0.1f);

			if (!Stay) {
				task.OnEnd = () => {
					Tween.To(Angle > 1 ? 0 : MaxAngle, Angle, x => Angle = x, 0.2f);
				};
			}
		}

		public override void Setup(JsonValue settings) {
			base.Setup(settings);

			Stay = settings["stay"].Bool(false);
			MaxAngle = settings["max_angle"].Number(180).ToRadians();
		}

		public static void RenderDebug(string id, JsonValue parent, JsonValue root) {
			AngledRenderer.RenderDebug(id, parent, root);

			var max = (float) root["max_angle"].AsNumber;

			if (ImGui.InputFloat("Max angle", ref max)) {
				root["max_angle"] = max;
			}
			
			var stay = root["stay"].AsBoolean;

			if (ImGui.Checkbox("Stay", ref stay)) {
				root["stay"] = stay;
			}
		}
	}
}