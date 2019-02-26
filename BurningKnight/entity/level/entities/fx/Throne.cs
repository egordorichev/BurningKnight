using BurningKnight.util;

namespace BurningKnight.entity.level.entities.fx {
	public class Throne : SaveableEntity {
		private static Animation Animations = Animation.Make("prop-throne", "-throne");
		private AnimationData Animation;

		public Throne() {
			_Init();
		}

		protected void _Init() {
			{
				W = 20;
				H = 33;
			}
		}

		public override void Init() {
			base.Init();
			Animation = Animations.Get("idle");
		}

		public override void Render() {
			base.Render();
			Animation.Render(this.X, this.Y, false);
		}
	}
}