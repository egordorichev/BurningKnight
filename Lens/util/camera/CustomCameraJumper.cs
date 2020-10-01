using Microsoft.Xna.Framework;

namespace Lens.util.camera {
	public interface CustomCameraJumper {
		Vector2 Jump(Camera.Target target);
	}
}