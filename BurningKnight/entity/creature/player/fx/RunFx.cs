using BurningKnight.util;

namespace BurningKnight.entity.creature.player.fx {
	public class RunFx : Entity {
		private static Animation Animations = Animation.Make("fx-run");
		private AnimationData Animation;

		public RunFx(float X, float Y) {
			this.X = X;
			this.Y = Y;
			Depth = -1;
			Animation = Animations.Get("idle");
		}

		public override void Update(float Dt) {
			if (Animation.Update(Dt)) Done = true;
		}

		public override void Render() {
			Animation.Render(this.X + 5, this.Y + 5, false, false, 5, 5, 0, 0.5f, 0.5f);
		}
	}
}