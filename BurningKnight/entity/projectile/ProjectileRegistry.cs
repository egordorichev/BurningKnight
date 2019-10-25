using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.mod;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.projectile.controller;
using BurningKnight.level.entities;
using BurningKnight.physics;
using Lens.input;

namespace BurningKnight.entity.projectile {
	public static class ProjectileRegistry {
		private static Dictionary<string, Action<Projectile>> registry = new Dictionary<string, Action<Projectile>>();

		public static void Add(string id, Action<Projectile> fn, Mod mod = null) {
			registry[$"{mod?.Prefix ?? Mods.BurningKnight}:{id}"] = fn;
		}

		public static Action<Projectile> Get(string id) {
			return registry.TryGetValue(id, out var f) ? f : null;
		}

		static ProjectileRegistry() {
			Add("disk", p => {
				CollisionFilterComponent.Add(p, (entity, with) => with is Mob ? CollisionResult.Disable : CollisionResult.Default);

				p.BounceLeft += 10;
				p.CanHitOwner = true;

				p.GetComponent<CircleBodyComponent>().Body.AngularVelocity = 10f;
			});
			
			Add("grenade", p => {
				CollisionFilterComponent.Add(p, (entity, with) => {
					if (with is Mob) {
						p.BounceLeft = 0;
						return CollisionResult.Enable;
					}
					
					return CollisionResult.Default;
				});
				
				p.Controller += SlowdownProjectileController.Make(1);
				p.BreaksFromWalls = false;
				
				p.OnDeath += (pr, t) => {
					ExplosionMaker.Make(pr, 32);
				};

				p.Controller += (pr, dt) => {
					if (pr.T >= 3f) {
						pr.Break();
					}
				};
			});
			
			Add("missile", p => {
				CollisionFilterComponent.Add(p, (entity, with) => {
					if (with is Mob) {
						p.BounceLeft = 0;
						return CollisionResult.Enable;
					}
					
					if (with is Prop) {
						return CollisionResult.Disable;
					}
					
					return CollisionResult.Default;
				});
				
				p.Controller += TargetProjectileController.Make(null, 0.5f);
				p.Controller += SmokeProjectileController.Make();
				
				p.OnDeath += (pr, t) => {
					ExplosionMaker.Make(pr, 32);
				};
			});
			
			Add("follower", p => {
				CollisionFilterComponent.Add(p, (entity, with) => {
					if (with is Mob) {
						p.BounceLeft = 0;
						return CollisionResult.Enable;
					} 
					
					if (with is Prop) {
						return CollisionResult.Disable;
					}

					return CollisionResult.Default;
				});
				
				p.Controller += TargetProjectileController.MakeCursor();
				p.Controller += SmokeProjectileController.Make();
				
				p.OnDeath += (pr, t) => {
					ExplosionMaker.Make(pr, 32);
				};
			});
			
			Add("flak", p => {
				CollisionFilterComponent.Add(p, (entity, with) => {
					if (with is Mob) {
						p.BounceLeft = 0;
						return CollisionResult.Enable;
					}
					
					return CollisionResult.Default;
				});
				
				p.Controller += SlowdownProjectileController.Make(0.5f);

				p.OnDeath += (pr, t) => {
					for (var i = 0; i < 8; i++) {
						Projectile.Make(pr.Owner, "default", (float) i / 8 * (float) Math.PI * 2, 8, true, 0, null, 0.8f).Center = pr.Center;
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
			
			Add("portal", p => {
				p.Center = Input.Mouse.GamePosition;
				p.GetAnyComponent<BodyComponent>().Velocity *= -1;
			});
		}
	}
}