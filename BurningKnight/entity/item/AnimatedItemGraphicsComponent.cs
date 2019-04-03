using BurningKnight.entity.component;
using Lens.assets;
using Lens.entity.component.graphics;
using Lens.graphics.animation;

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

		public override void Render() {
			if (!Entity.HasComponent<OwnerComponent>()) {
				Animation.Render(Entity.Position);
			}
		}
	}
}