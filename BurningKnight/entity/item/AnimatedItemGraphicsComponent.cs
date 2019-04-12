using BurningKnight.entity.component;
using Lens.assets;
using Lens.entity.component.graphics;
using Lens.graphics.animation;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.item {
	public class AnimatedItemGraphicsComponent : GraphicsComponent {
		public Animation Animation;
		
		public AnimatedItemGraphicsComponent(string animation) {
			Animation = Animations.Create(animation);
		}

		public override void Update(float dt) {
			base.Update(dt);
			Animation.Update(dt);
		}

		public override void Render(bool shadow) {
			if (!Entity.HasComponent<OwnerComponent>()) {
				if (shadow) {
					Animation?.Render(Entity.Position, Flipped, !FlippedVerticaly);
					return;
				}
				
				Animation.Render(Entity.Position);
			}
		}
	}
}