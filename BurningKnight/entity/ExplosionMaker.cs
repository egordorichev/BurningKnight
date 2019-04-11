using System;
using BurningKnight.assets.particle;
using BurningKnight.entity.component;
using BurningKnight.entity.events;
using BurningKnight.level.tile;
using BurningKnight.state;
using Lens;
using Lens.entity;
using Lens.util.camera;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity {
	public static class ExplosionMaker {
		public static void Make(Entity whoHurts, float hurtRadius, bool leave = true) {
			Camera.Instance.Shake(10);
				
			var explosion = new ParticleEntity(Particles.Animated("explosion", "explosion"));
			explosion.Particle.Position = whoHurts.Center;
			explosion.Depth = 32;
			whoHurts.Area.Add(explosion);

			for (int i = 0; i < 4; i++) {
				explosion = new ParticleEntity(Particles.Animated("explosion", "smoke"));
				explosion.Particle.Position = whoHurts.Center;
				explosion.Depth = 31;
				whoHurts.Area.Add(explosion);

				var a = explosion.Particle.Angle - Math.PI / 2;
				var d = 16;

				explosion.Particle.Position += new Vector2((float) Math.Cos(a) * d, (float) Math.Sin(a) * d);
			}
				
			Engine.Instance.Split = 1f;
			Engine.Instance.Flash = 1f;
			Engine.Instance.Freeze = 1f;
					
			foreach (var e in whoHurts.Area.GetEntitesInRadius(whoHurts.Center, hurtRadius, typeof(ExplodableComponent))) {
				e.GetComponent<ExplodableComponent>().HandleExplosion(whoHurts);
			}

			if (leave) {
				var xx = (int) Math.Floor(whoHurts.CenterX / 16f);
				var yy = (int) Math.Floor(whoHurts.CenterY / 16f);
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
								level.Set(index, Tile.FloorA);
								level.Set(index, Tile.Dirt);
								level.UpdateTile(x + xx, y + yy);
								level.CreateBody();

								whoHurts.HandleEvent(new SecretRoomFoundEvent {
									Who = whoHurts
								});
							}
						}
					}
				}
			}
		}
	}
}