using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.particle.custom {
	public class ProjectileParticle : Entity {
		private TextureRegion region;
		private float scale;
		private float angle;
		private Color color = new Color(0.5f, 0.5f, 1f, 1f);
		private Color secondColor = new Color(0.5f, 0.5f, 1f, 0.5f);
		
		public override void AddComponents() {
			base.AddComponents();

			AlwaysActive = true;

			scale = Random.Float(5, 8);
			angle = Random.AnglePI();
			region = CommonAse.Particles.GetSlice("fire");

			Width = scale;
			Height = scale;

			Depth = 0;
		}

		public override void Update(float dt) {
			base.Update(dt);

			scale -= dt * 10;
			angle += scale * dt * 6;
			
			if (scale <= 0) {
				Done = true;
			}
		}

		public override void Render() {
			Graphics.Color = secondColor;
			Graphics.Render(region, Position, angle, region.Center, new Vector2(scale));
			Graphics.Color = color;
			Graphics.Render(region, Position, angle, region.Center, new Vector2(scale * 0.5f));
			Graphics.Color = ColorUtils.WhiteColor;
		}
	}
}