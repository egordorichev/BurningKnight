using Lens.util.camera;

namespace BurningKnight.util {
	public static class AnimationUtil {
		public static void ActionFailed() {
			Camera.Instance.Shake();
			// todo: sfx
		}
	}
}