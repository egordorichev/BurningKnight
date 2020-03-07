using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.mod;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.projectile.controller;
using BurningKnight.level;
using BurningKnight.level.entities;
using BurningKnight.level.paintings;
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
			
			Add("shotgun", p => {
				p.Controller += SlowdownProjectileController.Make(1);
				p.BounceLeft += 1;
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
						var pr2 = Projectile.Make(pr.Owner, "shot", (float) i / 8 * (float) Math.PI * 2, 8, true, 0, null, 0.8f);
						pr2.Center = pr.Center;
						pr2.Controller += SlowdownProjectileController.Make(1);
					}
				};

				p.Controller += (pr, dt) => {
					if (pr.T >= 1f) {
						pr.Break();
					}
				};
			});
			
			Add("crash", p => {
				p.Controller += HsvProjectileController.Make();
				p.OnDeath += (pr, t) => {
					if (pr.T < 0.1f) {
						return;
					}
					
					for (var i = 0; i < 8; i++) {
						var p2 = Projectile.Make(pr.Owner, "square", (float) i / 8 * (float) Math.PI * 2, 8, true, 0, null, 0.8f);
						p2.Center = pr.Center;
						p2.Controller += HsvProjectileController.Make(1, pr.T);

						p2.OnDeath += (pr2, t2) => {
							if (pr2.T < 0.1f) {
								return;
							}
					
							for (var j = 0; j < 8; j++) {
								var p3 = Projectile.Make(pr.Owner, "square", (float) j / 8 * (float) Math.PI * 2, 12, true, 0, null, 0.6f);
								p3.Center = pr2.Center;
								p3.Controller += HsvProjectileController.Make(1, p2.T);
								
								p3.OnDeath += (pr4, t4) => {
									if (pr4.T < 0.1f) {
										return;
									}
					
									for (var k = 0; k < 8; k++) {
										var p5 = Projectile.Make(pr.Owner, "square", (float) k / 8 * (float) Math.PI * 2, 24, true, 0, null, 0.3f);
										p5.Center = pr4.Center;
										p5.Controller += HsvProjectileController.Make(1, pr4.T);
									}
								};
							}
						};
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
			
			Add("axe", p => {
				CollisionFilterComponent.Add(p, (entity, with) => ((with is Creature && with != p.Owner) || ((Projectile) entity).BounceLeft == 0) ? CollisionResult.Disable : CollisionResult.Default);

				p.DieOffscreen = false;
				p.Rotates = true;

				p.OnDeath = (pr, t) => {
					pr.Item.Renderer.Hidden = false;
				};

				p.OnCollision = (projectile, e) => {
					if (p.BounceLeft == 0) {
						if (e == projectile.Owner) {
							p.Break();
						} else if (!(e is Mob)) {
							return true;
						}
					} else if (projectile.BreaksFrom(e) && !(e is Painting || e is BreakableProp || e is ExplodingBarrel || e.HasComponent<HealthComponent>())) {
						var b = projectile.GetComponent<RectBodyComponent>().Body;
						b.LinearVelocity *= -1;
						
						projectile.BounceLeft = 0;
						projectile.EntitiesHurt.Clear();
						projectile.Controller += ReturnProjectileController.Make(projectile.Owner);
						return true;
					}
					
					return false;
				};
				
				p.BounceLeft = 1;
				p.Item.Renderer.Hidden = true;
			});
		}
	}
}