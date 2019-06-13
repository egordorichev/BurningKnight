using System;
using System.Collections.Generic;
using Lens.entity;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.projectile {
	public delegate void ProjectilePatternController(Projectile p, ProjectilePattern pt, int i, float dt);
	
	public class ProjectilePattern : Entity {
		private List<Data> projectiles = new List<Data>();
		private int maxProjectiles;
		
		public ProjectilePatternController Controller;
		public Vector2 Velocity;

		public int Count => maxProjectiles;
		public float T;
		
		public ProjectilePattern(ProjectilePatternController c) {
			Controller = c;
			AlwaysActive = true;
			Width = 0;
			Height = 0;
		}

		public void Launch(float angle, float speed) {
			Velocity = new Vector2((float) Math.Cos(angle) * speed, (float) Math.Sin(angle) * speed);
		}
		
		public override void Update(float dt) {
			base.Update(dt);

			T += dt;
			Position += Velocity * dt;
			
			if (Controller != null) {
				foreach (var p in projectiles) {
					Controller(p.Projectile, this, p.Id, dt);
				}
			}

			for (var i = projectiles.Count - 1; i >= 0; i--) {
				if (projectiles[i].Projectile.Done) {
					projectiles.RemoveAt(i);
				}
			}

			if (projectiles.Count == 0) {
				Done = true;
			}
		}

		public void Kill() {
			Done = true;

			foreach (var p in projectiles) {
				p.Projectile.Done = true;
			}
		}

		public void Add(Projectile p) {
			if (!p.Done) {
				projectiles.Add(new Data {
					Projectile = p,
					Id = maxProjectiles
				});
			}

			maxProjectiles++;
		}

		private class Data {
			public Projectile Projectile;
			public int Id;
		}
	}
}