using Lens.assets;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.particle.renderer {
	public class TexturedParticleRenderer : ParticleRenderer {
		public TextureRegion Region;

		public TexturedParticleRenderer(string slice) {
			Region = Animations.Get("particles").GetSlice(slice);
		}

		public TexturedParticleRenderer(TextureRegion r) {
			Region = r;
		}
		
		public TexturedParticleRenderer() {
			
		}

		public override void Render(Particle particle) {
			Graphics.Render(Region, particle.Position - new Vector2(0, particle.Z), particle.Angle, Region.Center, new Vector2(particle.Scale));
		}

		public override void RenderShadow(Particle particle) {
			if (particle.Z > 0.9f) {
				Graphics.Render(Region, particle.Position, particle.Angle, Region.Center, new Vector2(particle.Scale));
			}
		}
	}
}