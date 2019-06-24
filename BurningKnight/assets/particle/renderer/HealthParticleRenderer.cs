using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.particle.renderer {
	public class HealthParticleRenderer : TexturedParticleRenderer {
		private float scale;
		
		public HealthParticleRenderer(TextureRegion r, float sx) {
			Region = r;
			scale = sx;
		}

		public override void Render(Particle particle) {
			Graphics.Color = new Color(1f, 1f, 1f, particle.Alpha);
			Graphics.Render(Region, particle.Position, 0, Vector2.Zero, new Vector2(scale, 1));
			Graphics.Color = ColorUtils.WhiteColor;
		}
	}
}