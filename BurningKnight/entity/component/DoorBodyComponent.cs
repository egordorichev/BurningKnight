using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.component {
	public class DoorBodyComponent : RectBodyComponent {
		private bool lastReading;
		
		public DoorBodyComponent(float x, float y, float w, float h, BodyType type = BodyType.Dynamic, bool sensor = false, bool center = false) : base(x, y, w, h, type, sensor, center) {
			
		}

		public override void Update(float dt) {
			base.Update(dt);

			var sensor = !GetComponent<LockComponent>().Lock.IsLocked;
			
			//if (sensor != lastReading) {
				lastReading = sensor;
				Body.IsSensor = sensor;
			//}
		}
	}
}