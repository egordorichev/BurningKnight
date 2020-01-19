using Lens.graphics;
using Lens.graphics.animation;

namespace BurningKnight.ui {
	public class UiAnimation : UiEntity {
		public Animation Animation;
		public TextureRegion Slice;
		public bool UseSlice;

		public override void AddComponents() {
			base.AddComponents();

			Width = 48;
			Height = 48;
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (!UseSlice) {
				Animation?.Update(dt);
			}
		}

		public override void Render() {
			if (Animation == null && Slice == null) {
				return;
			}

			var region = UseSlice ? Slice : Animation.GetCurrentTexture();
			Graphics.Render(region, Center, 0, region.Center);
		}
	}
}