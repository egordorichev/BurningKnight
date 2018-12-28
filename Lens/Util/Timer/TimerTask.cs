using System;

namespace Lens.Util.Timer {
	public class TimerTask {
		public float Delay;
		public Action Fn;

		public TimerTask(Action fn, float delay) {
			Fn = fn;
			Delay = delay;
		}
	}
}