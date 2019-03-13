using Lens.assets;
using Lens.graphics;
using Lens.graphics.animation;

namespace Lens.entity.component.graphics {
	public class SliceComponent : GraphicsComponent {
		public TextureRegion Sprite;
		
		public SliceComponent(string image, string slice) {
			Sprite = Animations.Get(image).GetSlice(slice);
		}

		public SliceComponent(AnimationData image, string slice) {
			Sprite = image.GetSlice(slice);
		}

		public override void Render() {
			Graphics.Render(Sprite, Entity.Position);
		}
	}
}