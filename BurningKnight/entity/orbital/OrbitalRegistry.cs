using System;
using System.Collections.Generic;
using System.Linq;
using BurningKnight.assets;
using BurningKnight.assets.lighting;
using BurningKnight.assets.mod;
using BurningKnight.entity.component;
using BurningKnight.entity.creature;
using BurningKnight.entity.events;
using BurningKnight.entity.item.util;
using BurningKnight.entity.projectile;
using BurningKnight.state;
using BurningKnight.util;
using Lens.entity;
using Lens.util;
using Lens.util.math;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.orbital {
	public static class OrbitalRegistry {
		private static Dictionary<string, Func<Entity, Entity>> defined = new Dictionary<string, Func<Entity, Entity>>();

		public static Entity Create(string id, Entity owner) {
			return !defined.TryGetValue(id, out var d) ? null : d(owner);
		}
		
		public static void Define(string id, Func<Entity, Entity> orbital, Mod mod = null) {
			defined[$"{(mod == null ? Mods.BurningKnight : mod.Prefix)}:{id}"] = orbital;
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
				g.SetOwnerSize();

				orbital.AddComponent(new CircleBodyComponent(0, 0, 6, BodyType.Dynamic, true));
				
				orbital.OnCollision += (or, e) => {
					if (e is Projectile p && p.Owner != orbital.Owner) {
						p.Break();
					}
				};
				
				return orbital;
			});
			
			Define("bullet_stone", o => {
				var orbital = new Orbital();
				o.Area.Add(orbital);

				var g = new SliceComponent("items", "bk:bullet_stone") {
					ShadowZ = 2
				};

				orbital.AddComponent(g);
				g.AddShadow();
				g.SetOwnerSize();

				orbital.AddComponent(new CircleBodyComponent(0, 0, 6, BodyType.Dynamic, true));
				
				orbital.OnCollision += (or, e) => {
					if (e is Projectile p && p.Owner != orbital.Owner) {
						p.Break();
					}
				};

				var timer = 0f;

				orbital.Controller += (dt) => {
					timer += dt;

					if (timer >= 2f) {
						timer = 0;
						o.GetComponent<AudioEmitterComponent>().EmitRandomizedPrefixed("item_gun_fire", 2, 0.5f);

						var a = orbital.AngleTo(o.GetComponent<AimComponent>().RealAim);
						var projectile = Projectile.Make(o, "small", a, 10f);

						projectile.Color = ProjectileColor.Yellow;
						projectile.Center = orbital.Center + MathUtils.CreateVector(a, 5f);
						projectile.AddLight(32f, Projectile.YellowLight);
						
						o.HandleEvent(new ProjectileCreatedEvent {
							Projectile = projectile,
							Owner = o
						});
					}
				};
				
				return orbital;
			});
			
			Define("sword", o => {
				var orbital = new Orbital();
				o.Area.Add(orbital);

				var g = new SliceComponent("items", "bk:sword_orbital") {
					ShadowZ = 2
				};

				orbital.AddComponent(g);
				g.AddShadow();
				g.SetOwnerSize();

				orbital.AddComponent(new RectBodyComponent(0, 0, 9, 15, BodyType.Dynamic, true));
				
				orbital.OnCollision += (or, e) => {
					if (e is Creature c && c.IsFriendly() != ((Creature) orbital.Owner).IsFriendly()) {
						c.GetComponent<HealthComponent>().ModifyHealth(-1, orbital);
					} else if (e is Projectile p && p.Owner != orbital.Owner) {
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
				g.SetOwnerSize();
				
				orbital.AddComponent(new CircleBodyComponent(0, 0, 6, BodyType.Dynamic, true));
				
				orbital.OnCollision += (or, e) => {
					if (e is Projectile p && p.Owner != orbital.Owner) {
						p.Break();
						
						if (Rnd.Chance(20 - Run.Luck * 5)) {
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
				g.SetOwnerSize();

				orbital.AddComponent(new CircleBodyComponent(0, 0, 5, BodyType.Dynamic, true));
				
				orbital.OnCollision += (or, e) => {
					if (e is Projectile p && p.Owner != orbital.Owner) {
						p.Owner = o;

						var b = p.GetAnyComponent<BodyComponent>();
						var d = b.Velocity.Length();
						var a = b.Velocity.ToAngle() - Math.PI + Rnd.Float(-0.3f, 0.3f);

						b.Velocity = new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);
						
						if (p.TryGetComponent<LightComponent>(out var l)) {
							l.Light.Color = MeleeArc.ReflectedColor;
						}
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
				g.SetOwnerSize();

				orbital.AddComponent(new CircleBodyComponent(0, 0, 3, BodyType.Dynamic, true));
				
				orbital.OnCollision += (or, e) => {
					if (e is Projectile p && p.Owner != orbital.Owner) {
						p.Break();
					}
				};
				
				return orbital;
			});
			
			Define("planet", o => {
				var orbital = new Orbital();
				o.Area.Add(orbital);

				var g = new ScalableSliceComponent("items", "bk:earth") {
					ShadowZ = 2
				};

				orbital.AddComponent(g);
				g.AddShadow();
				g.SetOwnerSize();

				orbital.AddComponent(new CircleBodyComponent(0, 0, 3, BodyType.Dynamic, true));
				
				orbital.OnCollision += (or, e) => {
					if (e is Projectile p && p.Owner != orbital.Owner) {
						p.Break();
						var s = (ScalableSliceComponent) or.GraphicsComponent;

						if (Math.Abs(s.Scale.Y - 1) > 0.01f) {
							return; // Already animating
						}

						Tween.To(1, 1.5f, x => s.Scale.X = x, 0.3f);
						Tween.To(0, 1, x => s.Scale.Y = x, 0.2f).OnEnd = () => {
							s.Sprite = CommonAse.Items.GetSlice(planets[Rnd.Int(planets.Length)]);
							s.SetOwnerSize();
							
							orbital.RemoveComponent<CircleBodyComponent>();
							orbital.AddComponent(new CircleBodyComponent(0, 0, Math.Min(orbital.Width, orbital.Height) / 2f, BodyType.Dynamic, true));

							Tween.To(1, 1.5f, x => s.Scale.X = x, 0.4f, Ease.ElasticOut);
							Tween.To(1, 0, x => s.Scale.Y = x, 0.5f, Ease.ElasticOut);
						};
					}
				};
				
				return orbital;
			});
		}

		private static string[] planets = {
			"bk:mercury", "bk:venus", "bk:earth",
			"bk:moon", "bk:mars", "bk:jupiter",
			"bk:saturn", "bk:uranus", "bk:neptune"
		};
	}
}