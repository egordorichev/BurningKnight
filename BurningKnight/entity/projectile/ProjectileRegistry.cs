using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.projectile.controller;
using BurningKnight.physics;

namespace BurningKnight.entity.projectile {
	public static class ProjectileRegistry {
		private static Dictionary<string, Action<Projectile>> registry = new Dictionary<string, Action<Projectile>>();

		public static void Add(string id, Action<Projectile> fn, Mod mod = null) {
			registry[$"{mod?.GetPrefix() ?? Mods.BurningKnight}:{id}"] = fn;
		}

		public static Action<Projectile> Get(string id) {
			return registry.TryGetValue(id, out var f) ? f : null;
		}

		static ProjectileRegistry() {
			Add("grenade", p => {
				CollisionFilterComponent.Add(p, (entity, with) => with is Mob ? CollisionResult.Enable : CollisionResult.Default);
				
				p.Controller += SlowdownProjectileController.Make(1);
				p.BreaksFromWalls = false;
				
				p.OnDeath += (pr, t) => {
					ExplosionMaker.Make(pr);
				};

				p.Controller += (pr, dt) => {
					if (pr.T >= 3f) {
						pr.Break();
					}
				};
			});
		}
	}
}