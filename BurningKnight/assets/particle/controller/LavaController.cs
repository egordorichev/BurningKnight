namespace BurningKnight.assets.particle.controller {
	public class LavaController : ParticleController {
		public override bool Update(Particle particle, float dt) {
			particle.T += dt;
			particle.Position += particle.Velocity * dt;
			particle.Velocity.X -= particle.Velocity.X * dt * 4;
			particle.Velocity.Y += dt * 40;

			if (particle.Velocity.Y > 0) {
				particle.Scale -= dt * 0.5f;

				if (particle.Scale <= 0) {
					return true;
				}
			}

			return base.Update(particle, dt);
		}
	}
}