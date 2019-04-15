using BurningKnight.entity.component;
using Lens;
using Lens.graphics;

namespace BurningKnight.level.paintings {
	public class AnimatedPainting : Painting {
		public override void PostInit() {
			AddComponent(new AnimationComponent(Id));
			base.PostInit();
		}

		protected override TextureRegion GetRegion() {
			GetComponent<AnimationComponent>().Update(Engine.Delta);
			return GetComponent<AnimationComponent>().Animation.GetCurrentTexture();
		}
	}
}