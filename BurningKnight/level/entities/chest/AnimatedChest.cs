using BurningKnight.entity.component;
using Lens.util.tween;

namespace BurningKnight.level.entities.chest {
	public class AnimatedChest : Chest {
		protected override void AddGraphics() {
			AddComponent(new AnimationComponent(GetSprite()));
		}

		protected override void UpdateSprite() {
			GetComponent<AnimationComponent>().Animation.Tag = "open";
		}
		
		protected override void Animate() {
			var a = GetComponent<AnimationComponent>();

			a.Scale.X = 0.6f * Scale;
			a.Scale.Y = 1.7f * Scale;
					
			Tween.To(1.8f * Scale, a.Scale.X, x => a.Scale.X = x, 0.15f);
			Tween.To(0.2f * Scale, a.Scale.Y, x => a.Scale.Y = x, 0.15f).OnEnd = () => {
				Tween.To(Scale, a.Scale.X, x => a.Scale.X = x, 0.2f);
				Tween.To(Scale, a.Scale.Y, x => a.Scale.Y = x, 0.2f);
			};
		}

		protected override void AnimateOpening() {
			var a = GetComponent<AnimationComponent>();
					
			Tween.To(1.8f * Scale, a.Scale.X, x => a.Scale.X = x, 0.2f);
			Tween.To(0.2f * Scale, a.Scale.Y, x => a.Scale.Y = x, 0.2f).OnEnd = () => {
				DoOpening();
				
				Tween.To(0.6f * Scale, a.Scale.X, x => a.Scale.X = x, 0.1f);
				Tween.To(1.7f * Scale, a.Scale.Y, x => a.Scale.Y = x, 0.1f).OnEnd = () => {
					Tween.To(Scale, a.Scale.X, x => a.Scale.X = x, 0.2f);
					Tween.To(Scale, a.Scale.Y, x => a.Scale.Y = x, 0.2f);
				};
			};
		}
	}
}