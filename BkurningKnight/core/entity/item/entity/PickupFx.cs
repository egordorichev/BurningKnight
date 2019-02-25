using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.item.entity {
	public class PickupFx : Entity {
		public TextureRegion Region;
		public Point Target;
		private float Scale = 1f;
		private float T;

		public override Void Init() {
			this.AlwaysActive = true;
			this.AlwaysRender = true;
			this.Depth = 30;
			Tween.To(new Tween.Task(0, 1f, Tween.Type.LINEAR) {
				public override float GetValue() {
					return Scale;
				}

				public override Void SetValue(float Value) {
					Scale = Value;
				}

				public override Void OnEnd() {
					SetDone(true);
				}
			});
		}

		public override Void Update(float Dt) {
			this.T += Dt;
			float Dx = Target.X - this.X;
			float Dy = Target.Y - this.Y;
			float D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);

			if (D > 1) {
				this.X += Dx / (D / 2) * Dt * 50;
				this.Y += Dy / (D / 2) * Dt * 50;
			} 
		}

		public override Void Render() {
			Graphics.Render(Region, this.X + Region.GetRegionWidth() / 2, this.Y + Region.GetRegionHeight() / 2, this.T * 360f, Region.GetRegionWidth() / 2, Region.GetRegionHeight() / 2, false, false, Scale, Scale);
		}
	}
}
