using BurningKnight.entity.creature.bk;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.creature.mob.boss {
	public class BossPatternSet<T> where T : Boss {
		private string[] patterns;
		private BossPattern<T> current;
		private BossPatternRegistry<T> registry;

		public string[] Biomes;
		
		public BossPatternSet(BossPatternRegistry<T> r, params string[] p) {
			registry = r;
			patterns = p;
		}

		public BossAttack<T> GetNext() {
			if (current != null) {
				var attack = current.Generate();

				if (attack != null) {
					return attack;
				}

				current = null;
			}

			// fixme: i suspect that rng is not right here
			var i = Rnd.Int(patterns.Length);
			Log.Error($"{i} {patterns.Length}");
			current = registry.Get(patterns[i]);
			
			return GetNext();
		}
	}
}