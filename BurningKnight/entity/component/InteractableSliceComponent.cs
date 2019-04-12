using BurningKnight.assets;
using BurningKnight.util;
using Lens.entity.component.graphics;
using Lens.graphics;
using Lens.graphics.animation;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class InteractableSliceComponent : SliceComponent {
		public InteractableSliceComponent(string image, string slice) : base(image, slice) {}

		public InteractableSliceComponent(AnimationData image, string slice) : base(image, slice) {}

		public override void Render(bool shadow) {
			if (shadow) {
				Graphics.Render(Sprite, Entity.Position + new Vector2(0, Sprite.Height), 0, Vector2.Zero, Vector2.One, Graphics.ParseEffect(Flipped, !FlippedVerticaly));
				return;
			}
			
			if (Entity.TryGetComponent<InteractableComponent>(out var component) && component.OutlineAlpha > 0.05f) {
				var shader = Shaders.Entity;
				Shaders.Begin(shader);

				shader.Parameters["flash"].SetValue(component.OutlineAlpha);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(ColorUtils.White);

				foreach (var d in MathUtils.Directions) {
					Graphics.Render(Sprite, Entity.Position + d);
				}

				Shaders.End();
			}
			
			Graphics.Render(Sprite, Entity.Position);
		}
	}
}