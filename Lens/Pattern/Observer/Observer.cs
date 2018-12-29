namespace Lens.Pattern.Observer {
	public abstract class Observer {
		public abstract bool Observe(ObserverEvent message);
	}
}