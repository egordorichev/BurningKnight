using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.projectile.controller;
using BurningKnight.level.entities;
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
			Add("disk", p => {
				CollisionFilterComponent.Add(p, (entity, with) => with is Mob ? CollisionResult.Disable : CollisionResult.Default);

				p.BounceLeft += 10;
				p.CanHitOwner = true;

				p.GetComponent<CircleBodyComponent>().Body.AngularVelocity = 32f;

				// todo: spin
				// todo: hit player
			});
			
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
			
			// fixme: rect shape
			// todo: rotation
			Add("missile", p => {
				CollisionFilterComponent.Add(p, (entity, with) => with is Mob ? CollisionResult.Enable : CollisionResult.Default);
				
				p.Controller += TargetProjectileController.Make(null, 0.5f);
				
				p.OnDeath += (pr, t) => {
					ExplosionMaker.Make(pr);
				};
			});
			
			Add("flak", p => {
				CollisionFilterComponent.Add(p, (entity, with) => with is Mob ? CollisionResult.Enable : CollisionResult.Default);

				p.Controller += SlowdownProjectileController.Make(0.5f);

				p.OnDeath += (pr, t) => {
					for (var i = 0; i < 16; i++) {
						Projectile.Make(pr.Owner, "default", (float) i / 16 * (float) Math.PI * 2, 3, true, 0, null, 0.65f).Center = pr.Center;
					}
				};

				p.Controller += (pr, dt) => {
					if (pr.T >= 1f) {
						pr.Break();
					}
				};
			});
			
			Add("duck", p => {
				CollisionFilterComponent.Add(p, (entity, with) => with is Mob || with is Prop ? CollisionResult.Disable : CollisionResult.Default);
			});
		}
	}
}