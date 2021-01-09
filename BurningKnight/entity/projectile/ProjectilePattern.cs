using System;
using System.Collections.Generic;
using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.projectile {
	public delegate void ProjectilePatternController(Projectile p, ProjectilePattern.ProjectileData data, ProjectilePattern pt, int i, float dt);
	
	public class ProjectilePattern : Entity {
		public List<ProjectileData> Projectiles = new List<ProjectileData>();
		private int maxProjectiles;
		
		public ProjectilePatternController Controller;
		public Vector2 Velocity;

		public int Count => maxProjectiles;
		public float T;
		private bool launched;
		
		public ProjectilePattern(ProjectilePatternController c) {
			Controller = c;
			AlwaysActive = true;
			Width = 0;
			Height = 0;
		}

		public void Launch(float angle, float speed) {
			Velocity = new Vector2((float) Math.Cos(angle) * speed, (float) Math.Sin(angle) * speed);
			launched = true;
		}
		
		public override void Update(float dt) {
			base.Update(dt);

			if (launched) {
				T += dt;
				Position += Velocity * dt;

				if (Controller != null) {
					foreach (var p in Projectiles) {
						Controller(p.Projectile, p, this, p.Id, dt);
					}
				}
			}

			for (var i = Projectiles.Count - 1; i >= 0; i--) {
				if (Projectiles[i].Projectile.Done) {
					Projectiles.RemoveAt(i);
				}
			}

			if (launched) {
				if (Projectiles.Count == 0) {
					Done = true;
				}
			}
		}

		public void Kill() {
			Done = true;

			foreach (var p in Projectiles) {
				p.Projectile.Done = true;
			}
		}

		public void Add(Projectile p) {
			if (!p.Done) {
				Projectiles.Add(new ProjectileData {
					Projectile = p,
					Id = maxProjectiles,
					Distance = DistanceTo(p),
					Angle = AngleTo(p)
				});

				p.AddFlags(ProjectileFlags.ManualRotation);
			}

			maxProjectiles++;
		}

		public void Remove(Projectile p) {
			for (var i = 0; i < Projectiles.Count; i++) {
				if (Projectiles[i].Projectile == p) {
					Projectiles.RemoveAt(i);
					p.RemoveFlags(ProjectileFlags.ManualRotation);

					return;
				}
			}
		}

		public class ProjectileData {
			public Projectile Projectile;
			public int Id;
			public float Angle;
			public float Distance;
		}
	}
}