using BurningKnight.core.util;

namespace BurningKnight.core.entity.creature.player.fx {
	public class FootFx : Entity {
		private float T;
		private bool Fade;
		private float A;
		private float Angle;

		public FootFx(float X, float Y, float Angle, float A) {
			this.Angle = Angle;
			this.A = A;
			this.X = X;
			this.Y = Y;
			this.T = A * 3;
			this.Depth = -1;
		}

		public override Void Update(float Dt) {
			this.T += Dt;

			if (this.T > 3f && !this.Fade) {
				this.Fade = true;
				Tween.To(new Tween.Task(0, 3f) {
					public override float GetValue() {
						return A;
					}

					public override Void SetValue(float Value) {
						A = Value;
					}

					public override Void OnEnd() {
						Done = true;
					}
				});
			} 
		}

		public override Void Render() {
			Graphics.Batch.SetColor(1, 1, 1, this.A);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}
	}
}
