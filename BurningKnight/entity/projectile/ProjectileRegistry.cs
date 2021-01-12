using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.assets.mod;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.creature.mob;
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
			/*Add("skull", skull => {
				skull.NearDeath += p => {
					var c = new AudioEmitterComponent {
						DestroySounds = false
					};
							
					p.AddComponent(c);
					c.Emit("mob_oldking_explode");
				};
						
				skull.OnDeath += (p, e, t) => {
					for (var i = 0; i < 8; i++) {
						var bullet = Projectile.Make(p.Owner, "small", 
							((float) i) / 4 * (float) Math.PI, (i % 2 == 0 ? 2 : 1) * 4 + 3);
	
						bullet.CanBeReflected = false;
						bullet.Center = p.Center;
					}
				};

				skull.Controller += TargetProjectileController.Make(null, 0.5f);
				skull.Range = 5f;
				skull.IndicateDeath = true;
				skull.CanBeReflected = false;
				skull.GetComponent<ProjectileGraphicsComponent>().IgnoreRotation = true;
			});
		
			Add("disk", p => {
				CollisionFilterComponent.Add(p, (entity, with) => with is Mob || with is HalfProjectileLevel ? CollisionResult.Disable : CollisionResult.Default);

				p.BounceLeft += 10;
				p.CanHitOwner = true;

				p.GetComponent<CircleBodyComponent>().Body.AngularVelocity = 10f;
			});
			
			Add("what", p => {
				p.Controller += WhatController.Make();
				p.GetComponent<CircleBodyComponent>().Body.AngularVelocity = 10f;
			});
			
			Add("soap", p => {
				p.Controller += SlowdownProjectileController.Make(2);
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
				
				p.OnDeath += (pr, e, t) => {
					ExplosionMaker.Make(pr, 16, damage: 8);
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
				
				p.OnDeath += (pr, e, t) => {
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
				
				p.OnDeath += (pr, e, t) => {
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

				p.OnDeath += (pr, e, t) => {
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
				p.OnDeath += (pr, ee, t) => {
					if (pr.T < 0.1f) {
						return;
					}
					
					for (var i = 0; i < 8; i++) {
						var p2 = Projectile.Make(pr.Owner, "square", (float) i / 8 * (float) Math.PI * 2, 8, true, 0, null, 0.8f);
						p2.Center = pr.Center;
						p2.Controller += HsvProjectileController.Make(1, pr.T);

						p2.OnDeath += (pr2, eee, t2) => {
							if (pr2.T < 0.1f) {
								return;
							}
					
							for (var j = 0; j < 8; j++) {
								var p3 = Projectile.Make(pr.Owner, "square", (float) j / 8 * (float) Math.PI * 2, 12, true, 0, null, 0.6f);
								p3.Center = pr2.Center;
								p3.Controller += HsvProjectileController.Make(1, p2.T);
								
								p3.OnDeath += (pr4, eeee, t4) => {
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
				p.Center = p.Owner.GetComponent<CursorComponent>().Cursor.GamePosition;
				p.GetAnyComponent<BodyComponent>().Velocity *= -1;
			});
			
			Add("axe", p => {
				CollisionFilterComponent.Add(p, (entity, with) => ((with is Creature && with != p.Owner) || ((Projectile) entity).BounceLeft == 0) ? CollisionResult.Disable : CollisionResult.Default);

				var ts = Timer.Add(() => {
					p.Item.Renderer.Hidden = false;

					foreach (var u in p.Item.Uses) {
						if (u is SimpleShootUse ss) {
							ss.ProjectileDied = true;
							break;
						}
					}
				}, 3f);
				
				p.Range = 5;
				p.PreventSpectralBreak = true;

				p.OnCollision = (projectile, e) => {
					if (Run.Level.Biome is IceBiome && e is ProjectileLevelBody lvl) {
						if (lvl.Break(projectile.CenterX, projectile.CenterY)) {
							AudioEmitterComponent.Dummy(projectile.Area, projectile.Center).EmitRandomizedPrefixed("level_snow_break", 3);
						}
					}
					
					if (projectile.BounceLeft == 0) {
						if (e == projectile.Owner) {
							projectile.Item.Renderer.Hidden = false;
							projectile.Break();

							foreach (var u in projectile.Item.Uses) {
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
							projectile.BounceLeft++;
						} else {
							var b = projectile.GetComponent<RectBodyComponent>().Body;
							b.LinearVelocity *= -1;

							projectile.BounceLeft = 0;
							projectile.EntitiesHurt.Clear();
							projectile.Controller += ReturnProjectileController.Make(projectile.Owner);

							var pi = projectile.OnDeath;
							
							projectile.OnDeath = (pr, ee, t) => {
								pr.Item.Renderer.Hidden = false;
								
								foreach (var u in pr.Item.Uses) {
									if (u is SimpleShootUse ss) {
										ss.ProjectileDied = true;
										break;
									}
								}
								
								ts.Cancel();
								pr.Owner.GetComponent<AudioEmitterComponent>().EmitRandomized("item_axe_catch");
							};
							
							pi?.Invoke(projectile, e, false);
							return true;
						}
					}
					
					return false;
				};
				
				p.BounceLeft = 1;
				p.DieOffscreen = false;
				p.Rotates = true;

				p.Item.Renderer.Hidden = true;
			});
			
			Add("lava_wand", p => {
				CollisionFilterComponent.Add(p, (entity, with) => with is Mob || with is Prop ? CollisionResult.Disable : CollisionResult.Default);

				p.Rotates = true;
				p.OnDeath += (pr, e, t) => {
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
				};
			});

			Add("discord_rod", p => {
				CollisionFilterComponent.Add(p, (entity, with) => with is Mob || with is Prop ? CollisionResult.Disable : CollisionResult.Default);

				p.Rotates = true;
				p.OnDeath += (pr, e, t) => {
					pr.Owner.Center = pr.Center;
					Audio.PlaySfx("item_discord");
				};
			});
			
			Add("web_wand", p => {
				// CollisionFilterComponent.Add(p, (entity, with) => with is Mob || with is Prop ? CollisionResult.Disable : CollisionResult.Default);

				p.Rotates = true;
				p.OnDeath += (pr, e, t) => {
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
				};
			});*/
		}
	}
}