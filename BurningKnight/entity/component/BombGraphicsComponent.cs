using System;
using BurningKnight.assets;
using Lens.entity.component.graphics;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class BombGraphicsComponent : SliceComponent {
		public BombGraphicsComponent(string image, string slice) : base(image, slice) {
			
		}

		public override void Render(bool shadow) {
			var timer = Entity.GetComponent<ExplodeComponent>();
			var origin = new Vector2(Sprite.Center.X, shadow ? 0 : Sprite.Source.Height);
			var stopShader = false;
			
			if (!shadow && (timer.Timer < 1f ? timer.Timer % 0.3f > 0.1f : timer.Timer % 0.5f > 0.4f)) {
				var shader = Shaders.Entity;
				Shaders.Begin(shader);

				shader.Parameters["flash"].SetValue(1f);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(ColorUtils.White);
					
				stopShader = true;
			}

			Graphics.Render(Sprite, Entity.Position + new Vector2(origin.X, origin.Y + (shadow ? Sprite.Height : 0)),
				0, origin, new Vector2((float) (Math.Cos(timer.Timer * 16) / 2f) + 1, (float) (Math.Cos(timer.Timer * 16 + Math.PI) / 3f) + 1), 
				Graphics.ParseEffect(false, shadow));

			if (stopShader) {
				Shaders.End();
			}
		}
	}
}