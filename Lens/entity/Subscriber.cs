namespace Lens.entity {
	public interface Subscriber {
		void HandleEvent(Event e);
	}
}