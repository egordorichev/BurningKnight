using Lens.assets;
using Lens.graphics;
using Lens.graphics.animation;
using Lens.util;

namespace Lens.entity.component.graphics {
	public class SliceComponent : GraphicsComponent {
		public TextureRegion Sprite;
		
		public SliceComponent(string image, string slice) {
			Sprite = Animations.Get(image).GetSlice(slice);
			Check(slice);
		}

		public SliceComponent(AnimationData image, string slice) {
			Sprite = image.GetSlice(slice);
			Check(slice);
		}

		private void Check(string slice) {
			if (Sprite == null) {
				Log.Warning($"Unable to find slice {slice}");
			}
		}

		public override void Render() {
			Graphics.Render(Sprite, Entity.Position);
		}
	}
}