using BurningKnight.entity.component;
using VelcroPhysics.Dynamics;

namespace BurningKnight.entity.orbital {
	public class AnimatedOrbital : Orbital {
		private string sprite;

		public AnimatedOrbital(string spr) {
			sprite = spr;
		}
		
		public override void AddComponents() {
			base.AddComponents();
			
			AddComponent(new AnimationComponent(sprite) {
				ShadowOffset = -2
			});

			var region = GetComponent<AnimationComponent>().Animation.GetCurrentTexture();
			
			Width = region.Width;
			Height = region.Height;

			AddComponent(new ShadowComponent());
			AddComponent(new RectBodyComponent(0, 0, Width, Height, BodyType.Dynamic, true));
			
			GetComponent<AnimationComponent>().Animate();
		}
	}
}