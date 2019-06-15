using System;
using System.Collections.Generic;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.entity.projectile;
using BurningKnight.state;
using BurningKnight.util;
using Lens.entity;
using Lens.util;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;
using Random = Lens.util.math.Random;

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

				var g = new SliceComponent("items", "bk:goo") {
					ShadowZ = 2
				};

				orbital.AddComponent(g);
				g.AddShadow();

				orbital.AddComponent(new CircleBodyComponent(0, 0, 6, BodyType.Dynamic, true));
				
				orbital.OnCollision += (or, e) => {
					if (e is Projectile p) {
						p.Break();
					}
				};
				
				return orbital;
			});
			
			Define("broken_stone", o => {
				var orbital = new Orbital();
				o.Area.Add(orbital);
				
				var g = new SliceComponent("items", "bk:broken_stone") {
					ShadowZ = 2
				};
				
				orbital.AddComponent(g);
				g.AddShadow();
				
				orbital.AddComponent(new CircleBodyComponent(0, 0, 6, BodyType.Dynamic, true));
				
				orbital.OnCollision += (or, e) => {
					if (e is Projectile p) {
						p.Break();
						
						if (Random.Chance(20 - Run.Luck * 5)) {
							or.Done = true;
							AnimationUtil.Poof(or.Center);
						}
					}
				};
				
				return orbital;
			});
			
			Define("jelly", o => {
				var orbital = new Orbital();
				o.Area.Add(orbital);

				var g = new SliceComponent("items", "bk:jelly") {
					ShadowZ = 2
				};

				orbital.AddComponent(g);
				g.AddShadow();

				orbital.AddComponent(new CircleBodyComponent(0, 0, 5, BodyType.Dynamic, true));
				
				orbital.OnCollision += (or, e) => {
					if (e is Projectile p) {
						p.Owner = o;

						var b = p.GetAnyComponent<BodyComponent>();
						var d = b.Velocity.Length();
						var a = b.Velocity.ToAngle() - Math.PI + Random.Float(-0.3f, 0.3f);

						b.Velocity = new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);
					}
				};
				
				return orbital;
			});
			
			Define("nano_orb", o => {
				var orbital = new Orbital();
				o.Area.Add(orbital);

				var g = new SliceComponent("items", "bk:nano_orb") {
					ShadowZ = 2
				};

				orbital.AddComponent(g);
				g.AddShadow();

				orbital.AddComponent(new CircleBodyComponent(0, 0, 3, BodyType.Dynamic, true));
				
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