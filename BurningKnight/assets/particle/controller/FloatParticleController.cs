namespace BurningKnight.assets.particle.controller {
	public class FloatParticleController : ParticleController {
		public override bool Update(Particle particle, float dt) {
			particle.T += dt;
			particle.Position += particle.Velocity * dt;
			particle.Velocity -= particle.Velocity * dt * 2;
			
			if (particle.T >= 1.7f) {
				particle.Scale -= dt * 3;

				if (particle.Scale <= 0) {
					return true;
				}
			}
			
			return base.Update(particle, dt);
		}
	}
}