namespace Lens.Graphics.Animation {
	public enum AnimationDirection {
		Forward,
		Backwards,
		PingPong
	}

	static class AnimationDirectionMethosds {
		public static uint GetFrameId(this AnimationDirection direction, Animation animation) {
			switch (direction) {
				case AnimationDirection.Forward: return animation.StartFrame + animation.Frame;
				case AnimationDirection.Backwards: return animation.EndFrame - animation.Frame;
				default: return 0;
			}
		}
	}
}