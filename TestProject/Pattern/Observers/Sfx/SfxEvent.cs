using Lens.Asset;
using Lens.Pattern.Observer;

namespace TestProject.Pattern.Observers.Sfx {
	public class SfxEvent : ObserverEvent {
		private string sfxId;

		public SfxEvent(string sfx) {
			sfxId = sfx;
		}
		
		public override void Run() {
			Audio.PlaySfx(sfxId);	
		}
	}
}