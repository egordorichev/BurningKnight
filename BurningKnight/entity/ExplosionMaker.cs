using System;
using BurningKnight.assets.particle;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.level;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.util;
using Lens;
using Lens.entity;
using Lens.util.camera;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity {
	public static class ExplosionMaker {
		public static void Make(Entity whoHurts, float hurtRadius = 16f, bool leave = true, Vec2 where = null) {
			Camera.Instance.Shake(10);
			var w = where == null ? whoHurts.Center : new Vector2(where.X, where.Y);

			AnimationUtil.Explosion(w);
			
			for (int i = 0; i < 4; i++) {
				var explosion = new ParticleEntity(Particles.Animated("explosion", "smoke"));
				explosion.Position = w;
				whoHurts.Area.Add(explosion);
				explosion.Depth = 31;
				explosion.Particle.AngleVelocity = 0;
				explosion.AddShadow();

				var a = explosion.Particle.Angle - Math.PI / 2;
				var d = 16;

				explosion.Particle.Position += new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);
			}
			
			for (var i = 0; i < 6; i++) {
				var part = new ParticleEntity(Particles.Dust());
						
				part.Position = w + new Vector2(Random.Int(-4, 4), Random.Int(-4, 4));
				whoHurts.Area.Add(part);
				part.Depth = 30;
			}
			
			Engine.Instance.Split = 1f;
			Engine.Instance.Flash = 1f;
			Engine.Instance.Freeze = 1f;
					
			foreach (var e in whoHurts.Area.GetEntitesInRadius(w, hurtRadius, typeof(ExplodableComponent))) {
				e.GetAnyComponent<BodyComponent>()?.KnockbackFrom(whoHurts, 4f);
				e.GetComponent<ExplodableComponent>().HandleExplosion(whoHurts);
			}

			Camera.Instance.TextureZoom -= 0.05f;
			Tween.To(1f, Camera.Instance.TextureZoom, x => Camera.Instance.TextureZoom = x, 0.2f);
			
			if (leave) {
				whoHurts.Area.Add(new ExplosionLeftOver {
					Center = w
				});
				
				var xx = (int) Math.Floor(w.X / 16f);
				var yy = (int) Math.Floor(w.Y / 16f);
				var r = (int) Math.Floor(hurtRadius / 16f);
				var level = Run.Level;
				
				for (int x = -r; x <= r; x++) {
					for (int y = -r; y <= r; y++) {
						var xm = x * 16;
						var ym = y * 16;
						
						if (Math.Sqrt(xm * xm + ym * ym) <= hurtRadius) {
							var index = level.ToIndex(x + xx, y + yy);
							var tile = level.Get(index);
							
							if (!tile.IsWall()) {
								level.Set(index, Tile.Dirt);
								level.UpdateTile(x + xx, y + yy);
							} else if (tile == Tile.Crack) {
								DiscoverCrack(whoHurts, level, x + xx, y + yy);
							} else if (tile == Tile.Planks) {
								level.Set(index, Tile.Dirt);
								level.Destroyable.Break((x + xx) * 16, (y + yy) * 16);
							}
						}
					}
				}
			}
		}

		public static void DiscoverCrack(Entity who, Level level, int x, int y) {
			var index = level.ToIndex(x, y);
			
			level.Set(index, Tile.FloorA);
			level.Set(index, Tile.Dirt);
			level.UpdateTile(x, y);
			level.CreateBody();
			level.LoadPassable();
								
			who.HandleEvent(new SecretRoomFoundEvent {
				Who = who
			});

			LightUp(x * 16 + 8, y * 16 + 8);

			DestroyableLevel.Animate(who.Area, x, y);
		}

		public static void LightUp(float X, float Y) {
			var x = (int) (X / 16f);
			var y = (int) (Y / 16f);
			var d = 2;

			for (int xx = -d; xx <= d; xx++) {
				for (int yy = -d; yy <= d; yy++) {
					var ds = Math.Sqrt(xx * xx + yy * yy);

					if (ds <= d) {
						var level = Run.Level;
						var index = level.ToIndex(xx + x, yy + y);

						level.Light[index] = (float) Math.Max(level.Light[index], Math.Max(0.1f, (d - ds) / d));
					}
				}
			}
		}
	}
}