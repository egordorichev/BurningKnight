namespace Lens.graphics.animation {
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
				
				default: {
					uint frame;
					
					if (animation.PingGoingForward) {
						frame = animation.StartFrame + animation.Frame;

						if (frame == animation.EndFrame) {
							animation.PingGoingForward = false;
							animation.SkipNextFrame = true;
						}
					} else {
						frame = animation.EndFrame - animation.Frame;
						
						if (frame == animation.StartFrame) {
							animation.PingGoingForward = true;
							animation.SkipNextFrame = true;
						}
					}

					return frame;
				}
			}
		}
	}
}