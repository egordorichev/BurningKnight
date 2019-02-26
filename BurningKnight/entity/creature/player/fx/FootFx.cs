using BurningKnight.util;

namespace BurningKnight.entity.creature.player.fx {
	public class FootFx : Entity {
		private float A;
		private float Angle;
		private bool Fade;
		private float T;

		public FootFx(float X, float Y, float Angle, float A) {
			this.Angle = Angle;
			this.A = A;
			this.X = X;
			this.Y = Y;
			T = A * 3;
			Depth = -1;
		}

		public override void Update(float Dt) {
			T += Dt;

			if (T > 3f && !Fade) {
				Fade = true;
				Tween.To(new Tween.Task(0, 3f) {

		public override float GetValue() {
			return A;
		}

		public override void SetValue(float Value) {
			A = Value;
		}

		public override void OnEnd() {
			Done = true;
		}
	});
}

}
public override void Render() {
Graphics.Batch.SetColor(1, 1, 1, this.A);
Graphics.Batch.SetColor(1, 1, 1, 1);
}
}
}