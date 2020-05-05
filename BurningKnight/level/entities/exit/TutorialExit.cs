using BurningKnight.state;
using Lens.assets;

namespace BurningKnight.level.entities.exit {
	public class TutorialExit : Exit {
		protected override void Descend() {
			Run.GoToTutorial();
		}

		protected override string GetFxText() {
			return Locale.Get("tutorial");
		}
	}
}