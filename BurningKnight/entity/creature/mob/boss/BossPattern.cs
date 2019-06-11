using System;
using Lens.util;

namespace BurningKnight.entity.creature.mob.boss {
	public class BossPattern<T> where T : Boss {
		private Type[] attacks;
		private int currentAttack;

		public BossPattern(params Type[] pattern) {
			attacks = pattern;
		}
		
		public void Reset() {
			currentAttack = 0;
		}
		
		public BossAttack<T> Generate() {
			if (attacks.Length == 0) {
				Log.Error($"Boss pattern {GetType().Name} has 0 attacks!");
				return null;
			}

			if (currentAttack == attacks.Length) {
				Reset();
				return null;
			}
			
			return (BossAttack<T>) Activator.CreateInstance(attacks[currentAttack++]);
		}
	}
}