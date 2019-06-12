using BurningKnight.entity.creature.bk;
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
			
			current = registry.Get(patterns[Random.Int(patterns.Length)]);
			
			return GetNext();
		}
	}
}