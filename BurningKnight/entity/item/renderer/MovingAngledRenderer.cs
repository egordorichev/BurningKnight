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
	}
}