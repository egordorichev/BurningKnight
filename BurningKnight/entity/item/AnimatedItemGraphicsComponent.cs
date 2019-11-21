using BurningKnight.entity.component;
using Lens.assets;
using Lens.entity.component.graphics;
using Lens.graphics;
using Lens.graphics.animation;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item {
	public class AnimatedItemGraphicsComponent : GraphicsComponent {
		public Animation Animation;
		public float T;
		
		public AnimatedItemGraphicsComponent(string animation) {
			Animation = Animations.Create(animation);
			Animation.Randomize();
			T = Rnd.Float(32f);
		}

		public override void Update(float dt) {
			base.Update(dt);

			T += dt;
			Animation.Update(dt);
		}

		public override void Render(bool shadow) {
			if (!Entity.HasComponent<OwnerComponent>()) {
				if (shadow) {
					var region = Animation.GetCurrentTexture();
					Graphics.Render(region, Entity.Position, 0, new Vector2(0, -region.Height + 3f), Vector2.One, Graphics.ParseEffect(Flipped, !FlippedVerticaly));
					return;
				}
				
				Animation.Render(Entity.Position + new Vector2(0, ItemGraphicsComponent.CalculateMove(T * 1.2f) - 3f));
			}
		}
	}
}