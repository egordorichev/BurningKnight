using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.assets.particle.custom {
	public class FadingParticle : Entity {
		private TextureRegion region;
		private Vector2 scale = Vector2.One;
		private Vector4 color;

		public FadingParticle(TextureRegion sprite, Color tint) {
			region = sprite;

			Width = sprite.Width;
			Height = sprite.Height;

			color.X = tint.R / 255f;
			color.Y = tint.G / 255f;
			color.Z = tint.B / 255f;
			color.W = tint.A / 255f;
		}
		
		public override void Init() {
			base.Init();

			AlwaysActive = true;
		}

		public override void Update(float dt) {
			base.Update(dt);

			scale.X -= dt * 0.5f;
			scale.Y = scale.X;

			if (scale.X <= 0) {
				Done = true;
			}
		}

		public override void Render() {
			var shader = Shaders.Entity;
			Shaders.Begin(shader);

			shader.Parameters["flash"].SetValue(1f);
			shader.Parameters["flashReplace"].SetValue(1f);
			shader.Parameters["flashColor"].SetValue(color);
			
			Graphics.Render(region, Center, 0, region.Center, scale);

			Shaders.End();
		}
	}
}