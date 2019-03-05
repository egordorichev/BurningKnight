using Lens.util.camera;

namespace BurningKnight.util {
	public class FollowingDriver : CameraDriver {
		public override void Update(float dt) {
			base.Update(dt);

			foreach (var target in Camera.Targets) {
				if (target.Entity.Area == Camera.Area) {
					Camera.Position += target.Entity.Center * dt * target.Priority;
				} else {
					Camera.Approach(target.Entity.Center, dt * 5 * target.Priority);
				}
			}
		}
	}
}