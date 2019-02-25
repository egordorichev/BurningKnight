using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.entities.fx {
	public class Throne : SaveableEntity {
		protected void _Init() {
			{
				W = 20;
				H = 33;
			}
		}

		private static Animation Animations = Animation.Make("prop-throne", "-throne");
		private AnimationData Animation;

		public override Void Init() {
			base.Init();
			this.Animation = Animations.Get("idle");
		}

		public override Void Render() {
			base.Render();
			this.Animation.Render(this.X, this.Y, false);
		}

		public Throne() {
			_Init();
		}
	}
}
