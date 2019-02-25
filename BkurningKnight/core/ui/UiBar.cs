using BurningKnight.core.assets;
using BurningKnight.core.game.input;
using BurningKnight.core.util;

namespace BurningKnight.core.ui {
	public class UiBar : UiEntity {
		protected void _Init() {
			{
				IsSelectable = false;
			}
		}

		protected float Max;
		protected float Val;
		protected float Last;
		public bool Vertical;
		public bool Hovered;
		public TextureRegion R;

		public override Void Update(float Dt) {
			this.Hovered = (CollisionHelper.Check((int) Input.Instance.UiMouse.X, (int) Input.Instance.UiMouse.Y, (int) this.X, (int) this.Y, (int) this.W, (int) this.H));
		}

		public override Void Render() {
			Graphics.Batch.SetColor(0.1f, 0.1f, 0.1f, 1);
			Graphics.Batch.Draw(this.R, this.X, this.Y + (this.Vertical ? 0 : 1));

			if (this.Vertical) {
				Graphics.Batch.SetColor(0.5f, 0.5f, 0.5f, 1);
				this.Render2();
				Graphics.Batch.SetColor(1, 1, 1, 1);
				this.Render1();
			} else {
				Graphics.Batch.SetColor(0.5f, 0.5f, 0.5f, 1);
				this.Render1();
				Graphics.Batch.SetColor(1, 1, 1, 1);
				this.Render2();
			}

		}

		protected Void Render1() {
			int W = (int) Math.Ceil(this.Vertical ? this.W : this.W * (this.Last / this.Max));
			int H = (int) Math.Ceil(this.Vertical ? this.H * (this.Last / this.Max) : this.H);
			TextureRegion Region = new TextureRegion(this.R);
			Region.SetRegionX(Region.GetRegionX() + (int) (this.W - W));
			Region.SetRegionY(Region.GetRegionY() + (int) (this.H - H));
			Region.SetRegionWidth(W);
			Region.SetRegionHeight(H);
			Graphics.Batch.Draw(Region, this.X, this.Y);
		}

		protected Void Render2() {
			TextureRegion Region = new TextureRegion(this.R);
			int Ww = (int) Math.Ceil(this.Vertical ? this.W : this.W * (this.Val / this.Max));
			int Hh = (int) Math.Ceil(this.Vertical ? this.H * (this.Val / this.Max) : this.H);
			Region.SetRegion(this.R);
			Region.SetRegionX(Region.GetRegionX() + (int) (this.W - Ww));
			Region.SetRegionY(Region.GetRegionY() + (int) (this.H - Hh));
			Region.SetRegionWidth(Ww);
			Region.SetRegionHeight(Hh);
			Graphics.Batch.Draw(Region, this.X, this.Y);
		}

		public Void RenderInfo() {
			Graphics.Medium.Draw(Graphics.Batch, (int) this.Val + " / " + (int) this.Max, Input.Instance.UiMouse.X + 12, Input.Instance.UiMouse.Y + 6);
		}

		public Void SetValue(float V) {
			if (this.Vertical && V < this.Last) {
				this.Val = V;
				this.Last = V;

				return;
			} 

			if (V != this.Val) {
				this.Last = this.Val;
				Tween.To(new Tween.Task(V, 0.6f) {
					public override float GetValue() {
						return Last;
					}

					public override Void SetValue(float Value) {
						Last = Value;
					}
				});
			} 

			this.Val = V;
		}

		public Void SetMax(float Max) {
			this.Max = Max;
		}

		public UiBar() {
			_Init();
		}
	}
}
