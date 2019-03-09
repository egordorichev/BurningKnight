using System;
using Lens.entity.component.graphics;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.component {
	public class BombGraphicsComponent : SliceComponent {
		public BombGraphicsComponent(string image, string slice) : base(image, slice) {
			
		}

		public override void Render() {
			// todo: flashes and color changes with shaders
			
			var timer = Entity.GetComponent<ExplodeComponent>();
			var origin = new Vector2(Sprite.Center.X, Sprite.Source.Height);
			
			Graphics.Render(Sprite, Entity.Position + origin, 0, origin, new Vector2(
				(float) (Math.Cos(timer.Timer * 16) / 2f) + 1, (float) (Math.Cos(timer.Timer * 16 + Math.PI) / 3f) + 1)
			);
		}
	}
}