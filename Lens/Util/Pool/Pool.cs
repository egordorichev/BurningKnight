using System.Collections.Generic;

namespace Lens.Util.Pool {
	public abstract class Pool<T> {
		public List<T> All = new List<T>();
		private List<T> free = new List<T>();
		
		public T Get() {
			if (free.Count == 0) {
				var t = Create();
				All.Add(t);

				return t;
			}

			var tp = free[free.Count - 1];
			free.RemoveAt(free.Count - 1);
			
			return tp;
		}

		public void Free(T t) {
			free.Add(t);
		}

		public void Clear() {
			free.Clear();

			foreach (var p in All) {
				Destroy(p);
			}
			
			All.Clear();
		}

		public abstract T Create();

		public virtual void Destroy(T t) {
			
		}
	}
}