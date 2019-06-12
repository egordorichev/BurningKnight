using System.Collections.Generic;
using BurningKnight.entity.creature.bk.attacks;

namespace BurningKnight.entity.creature.mob.boss {
	public class BossPatternRegistry<T> where T : Boss {
		private Dictionary<string, BossPattern<T>> defined = new Dictionary<string, BossPattern<T>>();
		public int Count => defined.Count;
		
		public void Register(string id, BossPattern<T> pattern) {
			defined[id] = pattern;
		}

		public BossPattern<T> Get(string id) {
			return defined.TryGetValue(id, out var d) ? d : null;
		}
	}
}