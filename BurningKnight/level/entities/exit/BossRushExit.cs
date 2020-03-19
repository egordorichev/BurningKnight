using BurningKnight.state;
using Lens.assets;

namespace BurningKnight.level.entities.exit {
	public class BossRushExit : Exit {
		protected override void Descend() {
			Run.StartNew(1, RunType.BossRush);
		}

		protected override string GetFxText() {
			return Locale.Get("boss_rush");
		}
	}
}