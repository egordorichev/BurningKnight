using System.Collections.Generic;
using BurningKnight.assets.mod;

namespace Desktop.integration.twitch.happening {
	public static class HappeningRegistry {
		public static Dictionary<string, Happening> Defined = new Dictionary<string, Happening>();
		
		static HappeningRegistry() {
			Add("hurt", new HurtHappening());
			Add("big_hurt", new HurtHappening(2));
			Add("omega_hurt", new HurtHappening(3));
		}

		public static void Add(string id, Happening happening, Mod mod = null) {
			Defined[$"{mod?.Prefix ?? Mods.BurningKnight}:{id}"] = happening;
		}

		public static Happening Get(string id) {
			return Defined.TryGetValue(id, out var happening) ? happening : null;
		}
	}
}