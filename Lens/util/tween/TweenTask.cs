using System;
using System.Collections.Generic;

namespace Lens.util.tween {
	public class TweenTask {
		public Action OnStart;
		public Action OnEnd;
		public Action OnUpdate;
		public Func<float, float> EaseFn;

		public float Timer;
		public float Duration;
		public List<float> Start = new List<float>();
		public List<float> Range = new List<float>();
		public List<TweenValue> Vars = new List<TweenValue>();
		public float Delay;
		public bool Started;
		public bool Ended;
		public bool Single;
		public float From;
		public float To;
		public Action<float> Set;
		
		public void Update(float dt) {
			if (Delay >= 0) {
				Delay -= dt;

				if (Delay <= 0) {
					Timer = -Delay;
				} else {
					return;
				}
			}
			
			if (!Started) {
				Started = true;
				OnStart?.Invoke();
			}
			
			Timer += dt;
			float t = Timer / Duration;
			bool callEnd = false;
			
			if (Timer >= Duration) {
				Timer = Duration;
				t = 1f;
				callEnd = true;
			}

			if (EaseFn != null) {
				t = EaseFn(t);
			}

			try {
				Interpolate(t);
				OnUpdate?.Invoke();
			} catch (Exception e) {
				t = 1;
				
				if (EaseFn != null) {
					t = EaseFn(t);
				}
				
				Interpolate(t);

				callEnd = true;
				Log.Error(e);
			}

			if (callEnd && !Ended) {
				Ended = true;
				OnEnd?.Invoke();
			}
		}
		
		protected void Interpolate(float t) {
			if (Single) {
				Set(From + (To - From) * t);
				return;
			}
			
			
			int i = Vars.Count;
			
			while (i-- > 0) {
				Vars[i].Value = Start[i] + Range[i] * t;
			}
		}
	}
}