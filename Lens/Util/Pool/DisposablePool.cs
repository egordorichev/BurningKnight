using System;

namespace Lens.Util.Pool {
	public class DisposablePool<T> : Pool<T> where T: IDisposable {
		public override T Create() {
			throw new NotImplementedException();
		}

		public override void Destroy(T t) {
			t.Dispose();
		}
	}
}