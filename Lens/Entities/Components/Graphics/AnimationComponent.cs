using Lens.Asset;
using Lens.Graphics.Animation;

namespace Lens.Entities.Components.Graphics {
	public class AnimationComponent : GraphicsComponent {
		public Animation Animation;
		
		public AnimationComponent(string name, string layer = null) {
			var data = Animations.Get(name);

			if (data != null) {
				Animation = data.CreateAnimation();

				if (layer != null) {
					Animation.Layer = layer;
				}
			}
		}

		public override void Update(float dt) {
			base.Update(dt);
			Animation?.Update(dt);
		}

		public override void Render() {
			Animation?.Render(Entity.Position);
		}
	}
}