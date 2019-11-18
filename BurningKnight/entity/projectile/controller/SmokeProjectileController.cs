using BurningKnight.assets.particle;
using Lens.util.math;

namespace BurningKnight.entity.projectile.controller {
	public static class SmokeProjectileController {
		public static ProjectileUpdateCallback Make() {
			var t = 0f;
			
			return (p, dt) => {
				t += dt;

				if (t >= 0.1f) {
					t = 0;
					
					var part = new ParticleEntity(Particles.Dust());
					p.Area.Add(part);
					part.Particle.Position = p.Center;
					part.Particle.Velocity = p.BodyComponent.Velocity * -0.2f;
					part.Particle.Scale = Rnd.Float(0.6f, 0.8f);
					part.Depth = Layers.Creature;
				}
			};
		}
	}
}