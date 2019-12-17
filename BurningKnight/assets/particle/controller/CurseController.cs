namespace BurningKnight.assets.particle.controller {
	public class CurseController : SimpleParticleController {
		public override bool Update(Particle particle, float dt) {
			particle.T += dt;
			particle.Angle += particle.AngleVelocity * dt;
			particle.Position += particle.Velocity * dt;
			particle.Scale -= dt * 0.5f;

			if (particle.Scale <= 0f) {
				particle.Scale = 0;
				return true;
			}

			particle.AngleVelocity -= particle.AngleVelocity * dt * 4;
			particle.Velocity.X -= particle.Velocity.X * dt * 5;
			particle.Velocity.Y -= dt * 60f;
			
			return base.Update(particle, dt);
		}
	}
}