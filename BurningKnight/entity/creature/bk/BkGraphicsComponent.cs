using BurningKnight.assets;
using BurningKnight.entity.component;
using Lens.graphics;
using Lens.graphics.animation;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.bk {
	public class BkGraphicsComponent : AnimationComponent {
		private float t;
		public float Alpha = 1f;
	
		public BkGraphicsComponent(string animationName, string layer = null, string tag = null) : base(animationName, layer, tag) {
			
		}

		public override void Update(float dt) {
			base.Update(dt);
			t += dt;
		}

		public override void Render(bool shadow) {
			var pos = Entity.Position + Offset;

			if (shadow) {
				FlippedVerticaly = !FlippedVerticaly;
				pos.Y += Animation.GetCurrentTexture().Height - ShadowOffset * 2 + 4;
				Graphics.Color.A = (byte) (Alpha * 255);
			} else {
				var shader = Shaders.Bk;
				var region = Animation.GetCurrentTexture();

				shader.Parameters["pos"].SetValue(new Vector2(region.Source.X / (float) region.Texture.Width,
					region.Source.Y / (float) region.Texture.Height));
				shader.Parameters["size"].SetValue(new Vector2(region.Width / (float) region.Texture.Width,
					region.Height / (float) region.Texture.Height));
				shader.Parameters["time"].SetValue(t);
				shader.Parameters["a"].SetValue(Alpha);
				
				Shaders.Begin(shader);
			}

			CallRender(pos, shadow);

			if (shadow) {
				Graphics.Color.A = 255;
				FlippedVerticaly = !FlippedVerticaly;
			} else {
				Shaders.End();
			}
		}
	}
}