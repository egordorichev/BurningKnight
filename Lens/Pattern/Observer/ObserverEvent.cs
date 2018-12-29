using Lens.Entities;

namespace Lens.Pattern.Observer {
	public abstract class ObserverEvent {
		public Entity Entity;
		public abstract void Run();
	}
}