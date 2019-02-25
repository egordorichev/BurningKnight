using System;
using System.Collections.Generic;
using System.Reflection;

namespace Lens.util.tween {
	public static class Tween {
		private static List<TweenTask> tasks = new List<TweenTask>();
		
		public static TweenTask To<T>(T target, object values, float duration, Func<float, float> ease = null, float delay = 0) {
			if (ease == null) {
				// Default ease
				ease = Ease.QuadOut;
			}
			
			TweenTask task = new TweenTask();
			tasks.Add(task);
			
			task.Delay = delay;
			task.Duration = duration;
			task.EaseFn = ease;
			
			if (values == null) {
				return task;
			}
			
			foreach (PropertyInfo property in values.GetType().GetTypeInfo().DeclaredProperties) {
				var info = new TweenValue(target, property.Name);
				var to = Convert.ToSingle(new TweenValue(values, property.Name, false).Value);

				float s = Convert.ToSingle(info.Value);
				float r = to - s;

				task.Vars.Add(info);
				task.Start.Add(s);
				task.Range.Add(r);
			}
		
			return task;
		}

		public static void Update(float dt) {
			for (int i = tasks.Count - 1; i >= 0; i--) {
				var task = tasks[i];
				task.Update(dt);

				if (task.Ended) {
					tasks.RemoveAt(i);
				}
			}
		}
	}
}