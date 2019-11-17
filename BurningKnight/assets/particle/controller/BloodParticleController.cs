using Lens.entity;
using Lens.util.math;

namespace BurningKnight.assets.particle.controller {
	public class BloodParticleController : ParticleController {
		public override void Init(Particle particle, Entity owner) {
			base.Init(particle, owner);
			
			particle.AngleVelocity = Rnd.Float(0.6f, 1) * 5 * (Rnd.Chance() ? -1 : 1);
		}

		public override bool Update(Particle particle, float dt) {
			particle.T += dt;
			particle.Angle += particle.AngleVelocity * dt;
			particle.Position += particle.Velocity * dt;
			particle.Scale -= dt * 2f;

			if (particle.Scale <= 0f) {
				particle.Scale = 0;
				return true;
			}

			particle.Velocity.X -= particle.Velocity.X * dt;
			particle.Velocity.Y += dt * 256;
			
			return base.Update(particle, dt);
		}
	}
}