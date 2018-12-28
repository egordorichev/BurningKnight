namespace Lens.Util.Camera {
	public class FollowingDriver : CameraDriver {
		public override void Update(float dt) {
			base.Update(dt);
			
			if (Camera.Target == null) {
				return;
			}
			
			Camera.Approach(Camera.Target.Center, dt * 10);
		}
	}
}