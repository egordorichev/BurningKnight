using BurningKnight.entity;
using Lens;
using Lens.util.camera;

namespace BurningKnight.util {
	public class FollowingDriver : CameraDriver {
		public override void Update(float dt) {
			base.Update(dt);

			foreach (var target in Camera.Targets) {
				if (target.Entity.Area == Camera.Area) {
					Camera.PositionX += (target.Entity.CenterX - Display.UiWidth * 0.5f) * dt * target.Priority;
					Camera.PositionY += (target.Entity.CenterY - Display.UiHeight * 0.5f) * dt * target.Priority * Display.Viewport * 1.6f;	
				} else {
					Camera.Approach(target.Entity.Center, dt * 5 * target.Priority);
				}
			}
		}
	}
}