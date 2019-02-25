using BurningKnight.game.input;
using BurningKnight.util;

namespace BurningKnight.ui {
	public class UiBar : UiEntity {
		public bool Hovered;
		protected float Last;

		protected float Max;
		public TextureRegion R;
		protected float Val;
		public bool Vertical;

		protected void _Init() {
			{
				IsSelectable = false;
			}
		}

		public override void Update(float Dt) {
			Hovered = CollisionHelper.Check((int) Input.Instance.UiMouse.X, (int) Input.Instance.UiMouse.Y, (int) this.X, (int) this.Y, (int) W, (int) H);
		}

		public override void Render() {
			Graphics.Batch.SetColor(0.1f, 0.1f, 0.1f, 1);
			Graphics.Batch.Draw(R, this.X, this.Y + (Vertical ? 0 : 1));

			if (Vertical) {
				Graphics.Batch.SetColor(0.5f, 0.5f, 0.5f, 1);
				Render2();
				Graphics.Batch.SetColor(1, 1, 1, 1);
				Render1();
			}
			else {
				Graphics.Batch.SetColor(0.5f, 0.5f, 0.5f, 1);
				Render1();
				Graphics.Batch.SetColor(1, 1, 1, 1);
				Render2();
			}
		}

		protected void Render1() {
			var W = (int) Math.Ceil(Vertical ? this.W : this.W * (Last / Max));
			var H = (int) Math.Ceil(Vertical ? this.H * (Last / Max) : this.H);
			TextureRegion Region = new TextureRegion(R);
			Region.SetRegionX(Region.GetRegionX() + (int) (this.W - W));
			Region.SetRegionY(Region.GetRegionY() + (int) (this.H - H));
			Region.SetRegionWidth(W);
			Region.SetRegionHeight(H);
			Graphics.Batch.Draw(Region, this.X, this.Y);
		}

		protected void Render2() {
			TextureRegion Region = new TextureRegion(R);
			var Ww = (int) Math.Ceil(Vertical ? W : W * (Val / Max));
			var Hh = (int) Math.Ceil(Vertical ? H * (Val / Max) : H);
			Region.SetRegion(R);
			Region.SetRegionX(Region.GetRegionX() + (int) (W - Ww));
			Region.SetRegionY(Region.GetRegionY() + (int) (H - Hh));
			Region.SetRegionWidth(Ww);
			Region.SetRegionHeight(Hh);
			Graphics.Batch.Draw(Region, this.X, this.Y);
		}

		public void RenderInfo() {
			Graphics.Medium.Draw(Graphics.Batch, (int) Val + " / " + (int) Max, Input.Instance.UiMouse.X + 12, Input.Instance.UiMouse.Y + 6);
		}

		public void SetValue(float V) {
			if (Vertical && V < Last) {
				Val = V;
				Last = V;

				return;
			}

			if (V != Val) {
				Last = Val;
				Tween.To(new Tween.Task(V, 0.6f) {

		public override float GetValue() {
			return Last;
		}

		public override void SetValue(float Value) {
			Last = Value;
		}
	});
}

this.Val = V;

}
public void SetMax(float Max) {
this.Max = Max;
}
public UiBar() {
_Init();
}
}
}