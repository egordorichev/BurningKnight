using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using Lens.entity;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.orbital {
	public static class OrbitalRegistry {
		private static Dictionary<string, Func<Entity, Entity>> defined = new Dictionary<string, Func<Entity, Entity>>();

		public static Entity Create(string id, Entity owner) {
			return !defined.TryGetValue(id, out var d) ? null : d(owner);
		}

		public static void Define(string id, Func<Entity, Entity> orbital, Mod mod = null) {
			defined[$"{(mod == null ? Mods.BurningKnight : mod.GetPrefix())}:{id}"] = orbital;
		}

		public static bool Has(string id) {
			return defined.ContainsKey(id);
		}

		static OrbitalRegistry() {
			Define("goo", o => {
				var orbital = new Orbital();
				o.Area.Add(orbital);
				
				orbital.AddComponent(new SliceComponent("items", "bk:goo"));
				orbital.AddComponent(new CircleBodyComponent(0, 0, 6, BodyType.Dynamic, true));
				
				orbital.OnCollision += (or, e) => {
					if (e is Projectile p) {
						p.Break();
					}
				};
				
				return orbital;
			});
		}
	}
}