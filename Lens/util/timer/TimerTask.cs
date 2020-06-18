using System;

namespace Lens.util.timer {
	public class TimerTask {
		public float Delay;
		public Action Fn;

		public TimerTask(Action fn, float delay) {
			Fn = fn;
			Delay = delay;
		}
		
		public void Cancel() {
			Timer.Cancel(this);
		}
	}
}