namespace Lens.entity {
	public interface Subscriber {
		bool HandleEvent(Event e);
	}
}