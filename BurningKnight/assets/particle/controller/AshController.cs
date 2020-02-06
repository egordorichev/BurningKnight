namespace BurningKnight.assets.particle.controller {
	public class AshController : ParticleController {
		public override bool Update(Particle particle, float dt) {
			particle.T += dt;
			particle.Position += particle.Velocity * dt;
			particle.Velocity -= particle.Velocity * dt * 2;

			if (particle.T >= 0.3f) {
				particle.Scale -= dt * 1.5f;

				if (particle.Scale <= 0) {
					return true;
				}
			}

			return base.Update(particle, dt);
		}
	}
}