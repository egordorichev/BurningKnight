using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.component {
	public class SensorBodyComponent : RectBodyComponent {
		public SensorBodyComponent(float x, float y, float w, float h, BodyType type = BodyType.Dynamic, bool center = false) : base(x, y, w, h, type, true, center) {
			
		}

		public override void KnockbackFrom(float a, float force = 1, float rnd = 0) {
			
		}
	}
}