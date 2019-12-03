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
			Define("lil_boo", o => o.Area.Add(new LilBoo {
				Owner = o
			}));

			Define("strawberry", o => o.Area.Add(new FollowerPet("bk:strawberry") {
				Owner = o
			}));

			Define("meat_guy", o => {
				var timer = 0f;
				var pet = new FollowerPet("bk:meat_guy") {
					Owner = o
				};

				pet.Controller += dt => {
					timer += dt;

					if (timer >= 2f) {
						timer = 0;
						o.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("item_gun_fire", 2, 0.5f);
						
						var a = pet.AngleTo(o.GetComponent<AimComponent>().RealAim);
						var projectile = Projectile.Make(o, "default", a, 10f);

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
		}
	}
}