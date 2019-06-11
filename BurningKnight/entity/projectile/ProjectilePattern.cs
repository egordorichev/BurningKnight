using System.Collections.Generic;
using Lens.entity;

namespace BurningKnight.entity.projectile {
	public class ProjectilePattern : Entity {
		private List<Projectile> projectiles = new List<Projectile>();

		protected virtual void UpdateProjectile(Projectile projectile, int i, float dt) {
			
		}
		
		public override void Update(float dt) {
			base.Update(dt);

			for (var i = 0; i < projectiles.Count; i++) {
				UpdateProjectile(projectiles[i], i, dt);	
			}

			for (var i = projectiles.Count - 1; i >= 0; i--) {
				if (projectiles[i].Done) {
					projectiles.RemoveAt(i);
				}
			}

			if (projectiles.Count == 0) {
				Done = true;
			}
		}
	}
}