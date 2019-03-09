using System;
using Lens.entity.component.graphics;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item {
	public class ItemGraphicsComponent : SliceComponent {
		private float t;
		
		public ItemGraphicsComponent(string image, string slice) : base(image, slice) {
			
		}

		public override void Update(float dt) {
			base.Update(dt);
			t += dt;
		}

		public override void Render() {
			var origin = Sprite.Center;
			
			Graphics.Render(Sprite, Entity.Position + origin + new Vector2(0, (float) (Math.Sin(t * 2f) * 0.5f + 0.5f) * -6), (float) Math.Cos(t * 3f) * 0.4f, origin);
		}
	}
}