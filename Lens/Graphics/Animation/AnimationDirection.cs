namespace Lens.Graphics.Animation {
	public enum AnimationDirection {
		Forward = 0,
		Backwards = 1,
		PingPong = 2
	}

	static class AnimationDirectionMethods {
		public static uint GetFrameId(this AnimationDirection direction, Animation animation) {
			switch (direction) {
				case AnimationDirection.Forward: return animation.StartFrame + animation.Frame;
				case AnimationDirection.Backwards: return animation.EndFrame - animation.Frame;
				default: return 0; // FIXME: TODO
			}
		}
	}
}