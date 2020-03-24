using BurningKnight.state;
using Lens.assets;

namespace BurningKnight.level.entities.exit {
	public class TutorialExit : Exit {
		protected override void Descend() {
			Run.StartNew(-2);
		}

		protected override string GetFxText() {
			return Locale.Get("tutorial");
		}
	}
}