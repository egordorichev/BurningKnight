using Lens.entity;
using Lens.util.math;

namespace BurningKnight.assets.particle.controller {
	public class SparkParticleController : ParticleController {
		public override void Init(Particle particle, Entity owner) {
			base.Init(particle, owner);

			particle.AngleVelocity = Rnd.Sign() * Rnd.Float(1, 2);
			particle.Scale = 0;
			particle.Alpha = 1f;
			particle.T = 0;
		}

		public override bool Update(Particle particle, float dt) {
			particle.Angle += particle.AngleVelocity * dt;
			particle.T += dt;
			
			if (particle.T < 1f) {
				particle.Scale += dt;

				if (particle.Scale >= 1f) {
					particle.Scale = 1f;
				}
			} else {
				particle.AngleVelocity -= particle.AngleVelocity * dt;
				
				if (particle.T > 2f) {
					particle.Scale -= dt * 0.5f;

					if (particle.Scale <= 0) {
						return true;
					}
				}
			}
			
			return base.Update(particle, dt);
		}
	}
}