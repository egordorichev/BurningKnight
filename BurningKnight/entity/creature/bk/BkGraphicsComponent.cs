using BurningKnight.assets;
using BurningKnight.entity.component;
using Lens.graphics;
using Lens.graphics.animation;
using Lens.util;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.creature.bk {
	public class BkGraphicsComponent : AnimationComponent {
		private float t;
	
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
				pos.Y += Animation.GetCurrentTexture().Height - ShadowOffset * 2;
			}
			
			if (Entity.TryGetComponent<InteractableComponent>(out var component) && component.OutlineAlpha > 0.05f) {
				var shader = Shaders.Entity;
				Shaders.Begin(shader);

				shader.Parameters["flash"].SetValue(component.OutlineAlpha);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(ColorUtils.White);

				foreach (var d in MathUtils.Directions) {
					CallRender(pos + d, shadow);
				}
				
				Shaders.End();
			} else {
				var shader = Shaders.Bk; 
				var region = Animation.GetCurrentTexture();

				shader.Parameters["pos"].SetValue(new Vector2(region.Source.X / (float) region.Texture.Width, region.Source.Y / (float) region.Texture.Height));
				shader.Parameters["size"].SetValue(new Vector2(region.Width / (float) region.Texture.Width, region.Height / (float) region.Texture.Height));
				shader.Parameters["time"].SetValue(t);
				shader.Parameters["a"].SetValue(1f);
			}
			
			Graphics.Color = Tint;
			CallRender(pos, shadow);
			Graphics.Color = ColorUtils.WhiteColor;
				
			Shaders.End();
			
			if (shadow) {
				FlippedVerticaly = !FlippedVerticaly;
			}
		}
	}
}