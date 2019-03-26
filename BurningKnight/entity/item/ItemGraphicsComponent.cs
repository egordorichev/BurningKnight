using System;
using BurningKnight.assets;
using BurningKnight.entity.component;
using BurningKnight.util;
using Lens.entity.component.graphics;
using Lens.graphics;
using Microsoft.Xna.Framework;
using Random = Lens.util.math.Random;

namespace BurningKnight.entity.item {
	public class ItemGraphicsComponent : SliceComponent {
		private float t;
		
		public ItemGraphicsComponent(string slice) : base(CommonAse.Items, slice) {
			t = Random.Float(3f);
		}

		public override void Update(float dt) {
			base.Update(dt);
			t += dt;
		}

		public Vector2 CalculatePosition() {
			return Entity.Position + Sprite.Center + new Vector2(0, (float) (Math.Sin(t * 2f) * 0.5f + 0.5f) * -5.5f);
		}

		public override void Render() {
			var origin = Sprite.Center;
			var position = CalculatePosition();
			var angle = (float) Math.Cos(t * 3f) * 0.4f;
				
			if (Entity.TryGetComponent<InteractableComponent>(out var component) && component.OutlineAlpha > 0.05f) {
				var shader = Shaders.Entity;
				Shaders.Begin(shader);

				shader.Parameters["flash"].SetValue(component.OutlineAlpha);
				shader.Parameters["flashReplace"].SetValue(1f);
				shader.Parameters["flashColor"].SetValue(ColorUtils.White);

				foreach (var d in MathUtils.Directions) {
					Graphics.Render(Sprite, position + d, angle, origin);
				}
			}

			var sh = Shaders.Item;
			Shaders.Begin(sh);
			sh.Parameters["time"].SetValue(t * 0.1f);
			sh.Parameters["size"].SetValue(0.05f);
			
			Graphics.Render(Sprite, position, angle, origin);
			
			Shaders.End();
		}
	}
}