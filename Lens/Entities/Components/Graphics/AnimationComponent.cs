using Lens.Asset;
using Lens.Graphics.Animation;

namespace Lens.Entities.Components.Graphics {
	public class AnimationComponent : GraphicsComponent {
		public Animation Animation;
		private string name;
		
		public AnimationComponent(string animationName, string layer = null) {
			name = animationName;
			ReloadAnimation(layer);
		}

		private void ReloadAnimation(string layer = null) {
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

			if (Animations.Reload) {
				ReloadAnimation();
			}
			
			Animation?.Update(dt);
		}

		public override void Render() {
			Animation?.Render(Entity.Position);
		}
	}
}