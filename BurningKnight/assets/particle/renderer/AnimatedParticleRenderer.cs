using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.particle.renderer {
	public class AnimatedParticleRenderer : ParticleRenderer {
		public override void Render(Particle particle) {
			var part = (AnimatedParticle) particle;
			var region = part.Animation.GetCurrentTexture();
			
			Graphics.Render(region, part.Position, part.Angle, region.Center, new Vector2(particle.Scale));
		}

		public override void RenderShadow(Particle particle) {
			var part = (AnimatedParticle) particle;
			var region = part.Animation.GetCurrentTexture();
			
			Graphics.Render(region, part.Position + new Vector2(0, 6), part.Angle, region.Center, new Vector2(particle.Scale));
		}
	}
}