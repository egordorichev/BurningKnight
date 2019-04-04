using Lens.util;
using Lens.util.tween;

namespace BurningKnight.entity.item.renderer {
	public class MovingAngledRenderer : AngledRenderer {
		public float MaxAngle;
		public bool Stay;
		
		public MovingAngledRenderer(float maxAngle, bool stay = false) {
			MaxAngle = maxAngle.ToRadians();
			Stay = stay;
		}

		public override void OnUse() {
			Tween.To(Angle > 1 ? 0 : MaxAngle, Angle, x => Angle = x, 0.1f);
		}
	}
}