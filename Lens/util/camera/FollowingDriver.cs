namespace Lens.util.camera {
	public class FollowingDriver : CameraDriver {
		public override void Update(float dt) {
			base.Update(dt);

			foreach (var target in Camera.Targets) {
				Camera.Approach(target.Center, dt * 3);				
			}
		}
	}
}