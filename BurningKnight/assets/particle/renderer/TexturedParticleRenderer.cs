using Lens.assets;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.particle.renderer {
	public class TexturedParticleRenderer : ParticleRenderer {
		public TextureRegion Region;

		public TexturedParticleRenderer(string slice) {
			Region = Animations.Get("particles").GetSlice(slice);
		}
		
		public override void Render<T>(Particle particle) where T : AnimatedParticle {
			// todo: set alpha
			Graphics.Render(Region, particle.Position, particle.Alpha, Region.Center, new Vector2(particle.Scale));
		}
	}
}