using Lens.assets;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.particle.renderer {
	public class TexturedParticleRenderer : ParticleRenderer {
		public TextureRegion Region;

		public TexturedParticleRenderer(string slice) {
			Region = Animations.Get("particles").GetSlice(slice);
		}

		public override void Begin() {
			// todo: set the alpha
		}

		public override void End() {
			// todo: rmeove the lapha
		}

		public override void Render(Particle particle) {
			Graphics.Render(Region, particle.Position, particle.Alpha, Region.Center, new Vector2(particle.Scale));
		}
	}
}