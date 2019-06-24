namespace BurningKnight.assets.particle.controller {
	public class HealthParticleController : ParticleController {
		public override bool Update(Particle particle, float dt) {
			particle.Velocity.Y += dt * 256;
			particle.Position.Y += particle.Velocity.Y * dt;
			particle.Alpha -= dt;

			if (particle.Alpha <= 0) {
				return true;
			}
			
			return base.Update(particle, dt);
		}
	}
}