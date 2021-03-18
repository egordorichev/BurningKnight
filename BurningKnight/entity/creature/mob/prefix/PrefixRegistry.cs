using System;
using System.Collections.Generic;
using BurningKnight.assets.mod;

namespace BurningKnight.entity.creature.mob.prefix {
	public static class PrefixRegistry {
		public static Dictionary<string, Type> Defined = new Dictionary<string, Type>();

		static PrefixRegistry() {
			Define<ExplosivePrefix>("explosive");
			Define<DeathShotPrefix>("death_shot");
			Define<HealthyPrefix>("healthy_shot");
			// Define<EmeraldPrefix>("emerald");
			Define<FragilePrefix>("fragile");
			Define<GoldPrefix>("gold");
		}

		public static void Define<T>(string id, Mod mod = null) where T : Prefix {
			Defined[$"{(mod == null ? Mods.BurningKnight : mod.Prefix)}:{id}"] = typeof(T);
		}
	}
}