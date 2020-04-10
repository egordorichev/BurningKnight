using System;
using System.Collections.Generic;
using System.Linq;
using BurningKnight.assets;
using BurningKnight.assets.items;
using BurningKnight.assets.mod;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.projectile;
using Lens.entity;
using Lens.util;
using Lens.util.math;

namespace BurningKnight.entity.creature.pet {
	public static class PetRegistry {
		private static Dictionary<string, Func<Entity, Entity>> defined = new Dictionary<string, Func<Entity, Entity>>();

		public static Entity Create(string id, Entity owner) {
			return !defined.TryGetValue(id, out var d) ? null : d(owner);
		}
		
		public static Entity CreateRandom(Entity owner) {
			var keys = defined.Keys.ToArray();
			return Create(keys[Rnd.Int(keys.Length)], owner);
		}

		public static void Define(string id, Func<Entity, Entity> pet, Mod mod = null) {
			defined[$"{(mod == null ? Mods.BurningKnight : mod.Prefix)}:{id}"] = pet;
		}

		public static bool Has(string id) {
			return defined.ContainsKey(id);
		}

		static PetRegistry() {
			Define("backpack", o => o.Area.Add(new Backpack {
				Owner = o
			}));
			
			Define("crystal", o => o.Area.Add(new Crystal {
				Owner = o
			}));
			
			Define("lil_boo", o => o.Area.Add(new LilBoo {
				Owner = o
			}));

			Define("strawberry", o => o.Area.Add(new Strawberry() {
				Owner = o
			}));

			Define("snek", o => o.Area.Add(new SnekPet {
				Owner = o
			}));

			Define("meat_guy", o => {
				var timer = 0f;
				var pet = new AnimatedFollowerPet("meat_guy") {
					Owner = o
				};

				pet.Controller += dt => {
					timer += dt;

					if (timer >= 2f) {
						timer = 0;

						if ((o.GetComponent<RoomComponent>().Room?.Tagged[Tags.MustBeKilled].Count ?? 0) == 0) {
							return;
						}
						
						o.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("item_meatguy", 4, 0.5f);
						
						var a = pet.AngleTo(o.GetComponent<AimComponent>().RealAim);
						var projectile = Projectile.Make(o, "small", a, 10f);

						projectile.Color = ProjectileColor.Yellow;
						projectile.Center = pet.Center + MathUtils.CreateVector(a, 5f);
						projectile.AddLight(32f, Projectile.YellowLight);

						o.HandleEvent(new ProjectileCreatedEvent {
							Projectile = projectile,
							Owner = o
						});
					}
				};

				o.Area.Add(pet);
				return pet;
			});
			
			Define("skele_buddy", o => {
				var timer = 0f;
				var pet = new AnimatedFollowerPet("skele_buddy") {
					Owner = o
				};

				pet.Controller += dt => {
					timer += dt;

					if (timer >= 2f) {
						timer = 0;

						if ((o.GetComponent<RoomComponent>().Room?.Tagged[Tags.MustBeKilled].Count ?? 0) == 0) {
							return;
						}
						
						o.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("item_meatguy", 4, 0.5f);
						
						var a = pet.AngleTo(o.GetComponent<AimComponent>().RealAim) - Math.PI;

						for (var i = 0; i < 3; i++) {
							var projectile = Projectile.Make(o, "circle", a + (i - 1) * 0.3f + Rnd.Float(-0.1f, 0.1f), Rnd.Float(4, 6), scale: Rnd.Float(0.6f, 1f));

							projectile.Color = ProjectileColor.Red;
							projectile.Center = pet.Center + MathUtils.CreateVector(a, 5f);
							projectile.AddLight(32f, Projectile.RedLight);

							o.HandleEvent(new ProjectileCreatedEvent {
								Projectile = projectile,
								Owner = o
							});
						}
					}
				};

				o.Area.Add(pet);
				return pet;
			});

			Define("coin_pouch", o => o.Area.Add(new GeneratorPet("bk:coin_pouch", 3, a => Items.CreateAndAdd("bk:coin", a)) {
				Owner = o
			}));

			Define("key_pouch", o => o.Area.Add(new GeneratorPet("bk:key_pouch", 3, a => Items.CreateAndAdd("bk:key", a)) {
				Owner = o
			}));

			Define("bomb_pouch", o => o.Area.Add(new GeneratorPet("bk:bomb_pouch", 3, a => Items.CreateAndAdd("bk:bomb", a)) {
				Owner = o
			}));
			
			Define("batman", o => o.Area.Add(new GeneratorPet("bk:batman", 3, a => Items.CreateAndAdd("bk:battery", a)) {
				Owner = o
			}));

			Define("pouch_pouch", o => o.Area.Add(new GeneratorPet("bk:pouch_pouch", 4, a => Items.CreateAndAdd("bk:pouch", a)) {
				Owner = o
			}));

			Define("shield_pouch", o => o.Area.Add(new GeneratorPet("bk:shield_pouch", 8, a => Items.CreateAndAdd("bk:shield", a)) {
				Owner = o
			}));

			Define("shield_buddy", o => o.Area.Add(new ShieldBuddy() {
				Owner = o
			}));

			Define("wallet", o => o.Area.Add(new Wallet() {
				Owner = o
			}));

			Define("spiked_cookie", o => o.Area.Add(new SpikedCookie() {
				Owner = o
			}));

			Define("shooty", o => o.Area.Add(new Shooty() {
				Owner = o
			}));
		}
	}
}