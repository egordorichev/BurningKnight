using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.item.entity {
	public class PickupFx : Entity {
		public TextureRegion Region;
		private float Scale = 1f;
		private float T;
		public Point Target;

		public override void Init() {
			AlwaysActive = true;
			AlwaysRender = true;
			Depth = 30;
			Tween.To(new Tween.Task(0, 1f, Tween.Type.LINEAR) {

		public override float GetValue() {
			return Scale;
		}

		public override void SetValue(float Value) {
			Scale = Value;
		}

		public override void OnEnd() {
			SetDone(true);
		}
	});
}

public override void Update(float Dt) {
this.T += Dt;
float Dx = Target.X - this.X;
float Dy = Target.Y - this.Y;
float D = (float) Math.Sqrt(Dx * Dx + Dy * Dy);
if (D > 1) {
this.X += Dx / (D / 2) * Dt * 50;
this.Y += Dy / (D / 2) * Dt * 50;
}
}
public override void Render() {
Graphics.Render(Region, this.X + Region.GetRegionWidth() / 2, this.Y + Region.GetRegionHeight() / 2, this.T * 360f, Region.GetRegionWidth() / 2, Region.GetRegionHeight() / 2, false, false, Scale, Scale);
}
}
}