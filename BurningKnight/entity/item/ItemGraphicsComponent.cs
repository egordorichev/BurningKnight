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
		public const float FlashSize = 0.025f;
		
		public float T;
		
		public ItemGraphicsComponent(string slice) : base(CommonAse.Items, slice) {
			T = Random.Float(3f);
		}

		public override void Init() {
			base.Init();
			
			Entity.Width = Sprite.Source.Width;
			Entity.Height = Sprite.Source.Height;
		}

		public override void Update(float dt) {
			base.Update(dt);
			T += dt;
		}

		public virtual Vector2 CalculatePosition() {
			return Entity.Position + Sprite.Center + new Vector2(0, (float) (Math.Sin(T * 2f) * 0.5f + 0.5f) * -5.5f);
		}

		public override void Render() {
			if (Entity.HasComponent<OwnerComponent>()) {
				return;
			}
			
			var origin = Sprite.Center;
			var position = CalculatePosition();
			var angle = (float) Math.Cos(T * 1.2f) * 0.4f;
				
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
			sh.Parameters["time"].SetValue(T * 0.1f);
			sh.Parameters["size"].SetValue(FlashSize);
			
			Graphics.Render(Sprite, position, angle, origin);
			
			Shaders.End();
		}
	}
}