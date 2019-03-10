using Lens.graphics;

namespace BurningKnight.assets.particle.renderer {
	public class AnimatedParticleRenderer : ParticleRenderer {
		public override void Render(Particle particle) {
			var part = (AnimatedParticle) particle;
			var region = part.Animation.GetCurrentTexture();
			
			Graphics.Render(region, part.Position, part.Angle, region.Center);
		}
	}
}