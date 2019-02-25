using System;

namespace Lens.util.pool {
	public class DisposablePool<T> : Pool<T> where T: IDisposable {
		public override T Create() {
			throw new NotImplementedException();
		}

		public override void Destroy(T t) {
			t.Dispose();
		}
	}
}