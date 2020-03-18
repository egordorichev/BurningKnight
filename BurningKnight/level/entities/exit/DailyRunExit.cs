using BurningKnight.state;
using Lens.assets;

namespace BurningKnight.level.entities.exit {
	public class DailyRunExit : Exit {
		protected override void Descend() {
			Run.StartNew(1, RunType.Daily);
		}

		protected override string GetFxText() {
			return Locale.Get("daily_run");
		}
	}
}