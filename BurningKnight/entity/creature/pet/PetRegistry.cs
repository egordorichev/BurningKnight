using System;
using System.Collections.Generic;
using System.Linq;
using BurningKnight.assets;
using BurningKnight.assets.mod;
using Lens.entity;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.creature.pet {
	public static class PetRegistry {
		private static Dictionary<string, Func<Entity, Entity>> defined = new Dictionary<string, Func<Entity, Entity>>();

		public static Entity Create(string id, Entity owner) {
			return !defined.TryGetValue(id, out var d) ? null : d(owner);
		}
		
		public static Entity CreateRandom(Entity owner) {
			var keys = defined.Keys.ToArray();
			return Create(keys[Random.Int(keys.Length)], owner);
		}

		public static void Define(string id, Func<Entity, Entity> pet, Mod mod = null) {
			defined[$"{(mod == null ? Mods.BurningKnight : mod.Prefix)}:{id}"] = pet;
		}

		public static bool Has(string id) {
			return defined.ContainsKey(id);
		}

		static PetRegistry() {
			Define("lil_boo", o => o.Area.Add(new LilBoo {
				Owner = o
			}));

			Define("the_key", o => o.Area.Add(new TheKey {
				Owner = o
			}));

			Define("strawberry", o => o.Area.Add(new Strawberry {
				Owner = o
			}));
		}
	}
}