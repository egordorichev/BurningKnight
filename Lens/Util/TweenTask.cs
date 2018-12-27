using System;

namespace Lens.Util {
	public class TweenTask {
		private Action onStart;
		private Action onEnd;
		private Action onUpdate;
		private Func<float, float> ease = Ease.QuadOut;

		private float timer;
		private float speed;
		private float target;
		private float delay;
		private bool started;
		private bool ended;
		
		public void Update(float dt) {
			if (delay >= 0) {
				delay -= dt;

				if (delay <= 0) {
					timer = -delay;
					started = true;
					onStart?.Invoke();
				}
			}
			
			if (!started) {
				started = true;
				onStart?.Invoke();
			}
			
			timer += speed * dt;

			if (timer >= 1f) {
				timer = 1f;
				ended = true;
				// TODO: set value

				onEnd?.Invoke();
				return;
			}
			
			// TODO: set value
		}
	}
}