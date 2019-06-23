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
			var s = particle.Alpha <= 0.99f;

			if (s) {
				Graphics.Color = new Color(1f, 1 - particle.T, 0.25f - particle.T * 0.25f, particle.Alpha);	
			}

			Graphics.Render(Region, particle.Position - new Vector2(0, particle.Z), particle.Angle, Region.Center, new Vector2(particle.Scale));
			
			if (s) {
				Graphics.Color = ColorUtils.WhiteColor;
			}
		}

		public override void RenderShadow(Particle particle) {
			if (particle.Z > 0.9f) {
				Graphics.Render(Region, particle.Position, particle.Angle, Region.Center, new Vector2(particle.Scale));
			}
		}
	}
}