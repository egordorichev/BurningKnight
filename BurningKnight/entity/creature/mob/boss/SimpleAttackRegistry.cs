using Lens.util.math;

namespace BurningKnight.entity.creature.mob.boss {
	public class SimpleAttackRegistry<T> where T : Boss {
		private BossPattern<T>[] patterns;
		private BossPattern<T> current;

		public SimpleAttackRegistry(BossPattern<T>[] patterns) {
			this.patterns = patterns;
		}
		
		public BossAttack<T> Next() {
			if (current == null) {
				current = patterns[Rnd.Int(patterns.Length)];
			}

			var a = current.Generate();

			if (a == null) {
				current = null;
				return Next();
			}

			return a;
		}
	}
}