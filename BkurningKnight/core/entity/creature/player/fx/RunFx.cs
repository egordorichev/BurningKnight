using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.player.fx {
	public class RunFx : Entity {
		private static Animation Animations = Animation.Make("fx-run");
		private AnimationData Animation;

		public RunFx(float X, float Y) {
			this.X = X;
			this.Y = Y;
			this.Depth = -1;
			this.Animation = Animations.Get("idle");
		}

		public override Void Update(float Dt) {
			if (this.Animation.Update(Dt)) {
				this.Done = true;
			} 
		}

		public override Void Render() {
			this.Animation.Render(this.X + 5, this.Y + 5, false, false, 5, 5, 0, 0.5f, 0.5f);
		}
	}
}
