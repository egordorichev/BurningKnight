using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.mod;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.mob;
using BurningKnight.entity.item;
using BurningKnight.entity.item.use;
using BurningKnight.entity.projectile.controller;
using BurningKnight.level;
using BurningKnight.level.biome;
using BurningKnight.level.entities;
using BurningKnight.level.paintings;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.state;
using Lens.assets;
using Lens.entity;
using Lens.input;
using Lens.util.math;
using Lens.util.timer;

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
			Add("skull", skull => {
				ProjectileCallbacks.AttachDeathCallback(skull, (p, e, t) => {
					var b = new ProjectileBuilder(p.Owner, "small");
					b.RemoveFlags(ProjectileFlags.Reflectable, ProjectileFlags.BreakableByMelee);

					for (var i = 0; i < 8; i++) {
						var bullet = b.Shoot(((float) i) / 4 * (float) Math.PI, (i % 2 == 0 ? 2 : 1) * 4 + 3).Build();
						bullet.Center = p.Center;
					}
				});

				ProjectileCallbacks.AttachUpdateCallback(skull, TargetProjectileController.Make(null, 0.5f));

				skull.T = 5f;
				skull.RemoveFlags(ProjectileFlags.BreakableByMelee, ProjectileFlags.Reflectable);
				skull.GetComponent<ProjectileGraphicsComponent>().IgnoreRotation = true;
			});
		
			Add("disk", p => {
				CollisionFilterComponent.Add(p, (entity, with) => with is Mob || with is HalfProjectileLevel ? CollisionResult.Disable : CollisionResult.Default);

				p.Bounce += 10;
				p.AddFlags(ProjectileFlags.HitsOwner);

				p.GetComponent<CircleBodyComponent>().Body.AngularVelocity = 10f;
			});
			
			Add("what", p => {
				ProjectileCallbacks.AttachUpdateCallback(p, WhatController.Make());
				p.GetComponent<CircleBodyComponent>().Body.AngularVelocity = 10f;
			});
			
			Add("soap", p => {
				ProjectileCallbacks.AttachUpdateCallback(p, SlowdownProjectileController.Make(2));
			});

			Add("grenade", p => {
				CollisionFilterComponent.Add(p, (entity, with) => {
					if (with is Mob) {
						p.Bounce = 0;
						return CollisionResult.Enable;
					}
					
					return CollisionResult.Default;
				});

				ProjectileCallbacks.AttachUpdateCallback(p, SlowdownProjectileController.Make(1));

				ProjectileCallbacks.AttachDeathCallback(p, (pr, e, t) => {
					ExplosionMaker.Make(pr, 16, damage: 8);
				});

				ProjectileCallbacks.AttachUpdateCallback(p, (pr, dt) => {
					if (pr.T >= 3f) {
						pr.Break();
					}
				});
			});
			
			Add("missile", p => {
				CollisionFilterComponent.Add(p, (entity, with) => {
					if (with is Mob) {
						p.Bounce = 0;
						return CollisionResult.Enable;
					}
					
					if (with is Prop) {
						return CollisionResult.Disable;
					}
					
					return CollisionResult.Default;
				});

				ProjectileCallbacks.AttachUpdateCallback(p, TargetProjectileController.Make(null, 0.5f));
				ProjectileCallbacks.AttachUpdateCallback(p, SmokeProjectileController.Make());

				ProjectileCallbacks.AttachDeathCallback(p, (pr, e, t) => {
					ExplosionMaker.Make(pr, 32);
				});
			});
			
			Add("shotgun", p => {
				ProjectileCallbacks.AttachUpdateCallback(p, SlowdownProjectileController.Make(1));
				p.Bounce += 1;
			});
			
			Add("follower", p => {
				CollisionFilterComponent.Add(p, (entity, with) => {
					if (with is Mob) {
						p.Bounce = 0;
						return CollisionResult.Enable;
					} 
					
					if (with is Prop) {
						return CollisionResult.Disable;
					}

					return CollisionResult.Default;
				});

				ProjectileCallbacks.AttachUpdateCallback(p, TargetProjectileController.MakeCursor());
				ProjectileCallbacks.AttachUpdateCallback(p, SmokeProjectileController.Make());

				ProjectileCallbacks.AttachDeathCallback(p, (pr, e, t) => {
					ExplosionMaker.Make(pr, 32);
				});
			});
			
			Add("flak", p => {
				CollisionFilterComponent.Add(p, (entity, with) => {
					if (with is Mob) {
						p.Bounce = 0;
						return CollisionResult.Enable;
					}
					
					return CollisionResult.Default;
				});

				ProjectileCallbacks.AttachUpdateCallback(p, SlowdownProjectileController.Make(0.5f));

				ProjectileCallbacks.AttachDeathCallback(p, (pr, e, t) => {
					var b = new ProjectileBuilder(pr.Owner, "shot");

					for (var i = 0; i < 8; i++) {
						var pr2 = b.Shoot((float) i / 8 * (float) Math.PI * 2, 8).Build();
						pr2.Center = pr.Center;

						ProjectileCallbacks.AttachUpdateCallback(pr2, SlowdownProjectileController.Make(1));
					}
				});

				ProjectileCallbacks.AttachUpdateCallback(p, (pr, dt) => {
					if (pr.T >= 1f) {
						pr.Break();
					}
				});
			});

			Add("duck", p => {
				CollisionFilterComponent.Add(p, (entity, with) => with is Mob || with is Prop ? CollisionResult.Disable : CollisionResult.Default);
			});
			
			Add("portal", p => {
				p.Center = p.Owner.GetComponent<CursorComponent>().Cursor.GamePosition;
				p.GetAnyComponent<BodyComponent>().Velocity *= -1;
			});
			
			Add("axe", p => {
				CollisionFilterComponent.Add(p, (entity, with) => ((with is Creature && with != p.Owner) || ((Projectile) entity).Bounce == 0) ? CollisionResult.Disable : CollisionResult.Default);

				var ts = Timer.Add(() => {
					((Item) p.Owner).Renderer.Hidden = false;

					foreach (var u in ((Item) p.Owner).Uses) {
						if (u is SimpleShootUse ss) {
							ss.ProjectileDied = true;
							break;
						}
					}
				}, 3f);
				
				p.T = 5;

				ProjectileCallbacks.AttachCollisionCallback(p, (projectile, e) => {
					if (Run.Level.Biome is IceBiome && e is ProjectileLevelBody lvl) {
						if (lvl.Break(projectile.CenterX, projectile.CenterY)) {
							AudioEmitterComponent.Dummy(projectile.Area, projectile.Center).EmitRandomizedPrefixed("level_snow_break", 3);
						}
					}
					
					if (projectile.Bounce == 0) {
						if (e == projectile.Owner) {
							((Item) projectile.Owner).Renderer.Hidden = false;
							projectile.Break();

							foreach (var u in ((Item) projectile.Owner).Uses) {
								if (u is SimpleShootUse ss) {
									ss.ProjectileDied = true;
									break;
								}
							}
						} else if (!(e is Mob)) {
							return true;
						}
					} else if (projectile.BreaksFrom(e, null)) {
						if (e is Painting || e is BreakableProp || e is ExplodingBarrel || e.HasComponent<HealthComponent>()) {
							projectile.Bounce++;
						} else {
							var b = projectile.GetComponent<RectBodyComponent>().Body;
							b.LinearVelocity *= -1;

							projectile.Bounce = 0;
							projectile.EntitiesHurt.Clear();
							ProjectileCallbacks.AttachUpdateCallback(projectile, ReturnProjectileController.Make(projectile.Owner));

							var pi = projectile.Callbacks?.OnDeath;

							ProjectileCallbacks.AttachDeathCallback(projectile, (pr, ee, t) => {
								((Item) pr.Owner).Renderer.Hidden = false;
								
								foreach (var u in ((Item) pr.Owner).Uses) {
									if (u is SimpleShootUse ss) {
										ss.ProjectileDied = true;
										break;
									}
								}
								
								ts.Cancel();
								pr.Owner.GetComponent<AudioEmitterComponent>().EmitRandomized("item_axe_catch");
							});
							
							pi?.Invoke(projectile, e, false);
							return true;
						}
					}
					
					return false;
				});
				
				p.Bounce = 1;
				p.AddFlags(ProjectileFlags.AutomaticRotation);

				((Item) p.Owner).Renderer.Hidden = true;
			});
			
			Add("lava_wand", p => {
				CollisionFilterComponent.Add(p, (entity, with) => with is Mob || with is Prop ? CollisionResult.Disable : CollisionResult.Default);

				p.AddFlags(ProjectileFlags.AutomaticRotation);

				ProjectileCallbacks.AttachDeathCallback(p, (pr, e, t) => {
					AudioEmitterComponent.Dummy(pr.Area, pr.Center).EmitRandomized("item_magic_lava_appear");
				
					var x = (int) Math.Round(pr.CenterX / 16f);
					var y = (int) Math.Round(pr.CenterY / 16f);
					const float r = 2.3f;

					for (var xx = -r; xx <= r; xx++) {
						for (var yy = -r; yy <= r; yy++) {
							var zx = (int) xx + x;
							var zy = (int) yy + y;
							
							if (Math.Sqrt(xx * xx + yy * yy) <= r && Run.Level.Get(zx, zy).IsPassable()) {
								Run.Level.Set(zx, zy, Tile.Lava);

								Timer.Add(() => {
									Run.Level.Set(zx, zy, Tile.Ember);
									Run.Level.UpdateTile(zx, zy);
								}, Rnd.Float(5f, 15f));
							}
						}
					}
					
					Run.Level.TileUp();
				});
			});

			Add("discord_rod", p => {
				CollisionFilterComponent.Add(p, (entity, with) => with is Mob || with is Prop ? CollisionResult.Disable : CollisionResult.Default);
				p.AddFlags(ProjectileFlags.AutomaticRotation);

				ProjectileCallbacks.AttachDeathCallback(p, (pr, e, t) => {
					pr.Owner.Center = pr.Center;
					Audio.PlaySfx("item_discord");
				});
			});
			
			Add("web_wand", p => {
				// CollisionFilterComponent.Add(p, (entity, with) => with is Mob || with is Prop ? CollisionResult.Disable : CollisionResult.Default);

				p.AddFlags(ProjectileFlags.AutomaticRotation);
				ProjectileCallbacks.AttachDeathCallback(p, (pr, e, t) => {
					AudioEmitterComponent.Dummy(pr.Area, pr.Center).EmitRandomized("item_magic_web_appear");

					var x = (int) Math.Round(pr.CenterX / 16f);
					var y = (int) Math.Round(pr.CenterY / 16f);
					const float r = 2.3f;

					for (var xx = -r; xx <= r; xx++) {
						for (var yy = -r; yy <= r; yy++) {
							var zx = (int) xx + x;
							var zy = (int) yy + y;
							
							if (Math.Sqrt(xx * xx + yy * yy) <= r && Run.Level.Get(zx, zy).IsPassable()) {
								Run.Level.Set(zx, zy, Tile.Cobweb);
							}
						}
					}
					
					Run.Level.TileUp();
				});
			});
		}
	}
}