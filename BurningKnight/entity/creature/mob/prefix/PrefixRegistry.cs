using System;
using System.Collections.Generic;

namespace BurningKnight.entity.creature.mob.prefix {
	public static class PrefixRegistry {
		public static Dictionary<string, Type> Defined = new Dictionary<string, Type>();

		static PrefixRegistry() {
			Define<ExplosivePrefix>("bk:explosive");
		}

		public static void Define<T>(string id) where T : Prefix {
			Defined[id] = typeof(T);
		}
	}
}