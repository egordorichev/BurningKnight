using Lens.Pattern.Observer;

namespace TestProject.Pattern.Observers.Sfx {
	public class SfxObserver : Observer {
		public override bool Observe(ObserverEvent message) {
			if (message is SfxEvent) {
				message.Run();
				return true;
			}

			return false;
		}
	}
}