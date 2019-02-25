using BurningKnight.core.assets;
using BurningKnight.core.entity;
using BurningKnight.core.game.input;
using BurningKnight.core.game.state;
using BurningKnight.core.util;

namespace BurningKnight.core.ui {
	public class UiSlider : UiButton {
		private float Min;
		private float Max;
		protected float Val;
		private float Sw;
		private static TextureRegion Slider = Graphics.GetTexture("ui-slider");
		private static TextureRegion Fill = Graphics.GetTexture("ui-slider_fill");
		private float Ox;

		public UiSlider(string Label, int X, int Y) {
			base(Label, X, Y, true);
			this.Ox = X;
			this.Min = 0;
			this.Max = 1;
			this.Val = 1;
		}

		public UiSlider SetValue(float Val) {
			this.Val = Val;

			return this;
		}

		public override Void SetLabel(string Label) {
			base.SetLabel(Label);
			this.X += this.W / 2;
			this.Sw = Slider.GetRegionWidth() + 8;
			this.W += this.Sw;
			UiChoice.MaxW = Math.Max(W, UiChoice.MaxW);
		}

		public override Void Render() {
			Graphics.Batch.End();
			Graphics.Shadows.End();
			Graphics.Text.Begin();
			Graphics.Batch.Begin();
			Graphics.Batch.SetProjectionMatrix(Camera.Nil.Combined);
			Gdx.Gl.GlClearColor(0, 0, 0, 0);
			Gdx.Gl.GlClear(GL20.GL_COLOR_BUFFER_BIT | GL20.GL_DEPTH_BUFFER_BIT | (Gdx.Graphics.GetBufferFormat().CoverageSampling ? GL20.GL_COVERAGE_BUFFER_BIT_NV : 0));
			Graphics.Medium.SetColor(this.R * this.Ar, this.G * this.Ag, this.B * this.Ab, 1);
			Graphics.Medium.Draw(Graphics.Batch, this.Label, 2, 16);
			float W = UiChoice.MaxW;
			Graphics.Medium.Draw(Graphics.Batch, this.Label, this.Ox - (W) * 0.5f + 4, this.Y - this.H / 2 + 16);
			Graphics.Medium.SetColor(1, 1, 1, 1);
			W = Slider.GetRegionWidth() - 4;
			float V = MathUtils.Map(this.Val, this.Min, this.Max, 0, W / 4);
			float Vl = 0.2f;
			Graphics.Batch.SetColor(Vl, Vl, Vl, 1);
			Graphics.Render(Fill, UiChoice.MaxW - W + 2, H * 0.5f + 2, 0, 0, 0, false, false, W / 4 * Scale, Scale * 1.1f);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			Graphics.Render(Fill, UiChoice.MaxW - W + 2, H * 0.5f + 2, 0, 0, 0, false, false, V * Scale, Scale * 1.1f);
			Graphics.Render(Slider, UiChoice.MaxW - W, H * 0.5f, 0, 0, 0, false, false, Scale, Scale);
			string S = ((int) Math.Floor((this.Val) * 100f)) + "%";
			Graphics.Layout.SetText(Graphics.Small, S);
			Graphics.Print(S, Graphics.Small, UiChoice.MaxW - W * 0.5f - Graphics.Layout.Width / 2, H * 0.5f + 2);
			Graphics.Batch.End();
			Graphics.Text.End();
			Graphics.Shadows.Begin();
			Graphics.Batch.Begin();
			Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
			Texture Texture = Graphics.Text.GetColorBufferTexture();
			Texture.SetFilter(Texture.TextureFilter.Nearest, Texture.TextureFilter.Nearest);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			Graphics.Batch.Draw(Texture, this.X - UiChoice.MaxW / 2 + 2, this.Y - this.H / 2, this.W / 2, this.H / 2 + 8, this.W + 72, this.H + 16, this.Scale, this.Scale, (float) (Math.Cos(this.Y / 12 + Dungeon.Time * 6) * (this.Mx / UiChoice.MaxW * 10 + 1f)), 0, 0, this.W + 72, this.H + 16, false, true);
		}

		public override Void Update(float Dt) {
			base.Update(Dt);

			if ((this.IsSelected || this.Hover)) {
				if (Input.Instance.WasPressed("ui_left")) {
					this.Val = MathUtils.Clamp(this.Min, this.Max, this.Val - (this.Max - this.Min) / 16);
					Audio.PlaySfx("menu/moving");
					this.OnUpdate();
				} else if (Input.Instance.WasPressed("ui_right")) {
					this.Val = MathUtils.Clamp(this.Min, this.Max, this.Val + (this.Max - this.Min) / 16);
					Audio.PlaySfx("menu/moving");
					this.OnUpdate();
				} 

				if (this.CheckHover()) {
					if ((Input.Instance.IsDown("mouse"))) {
						float Prev = this.Val;
						this.Val = MathUtils.Clamp(this.Min, this.Max, MathUtils.Map(Input.Instance.UiMouse.X + InGameState.SettingsX - (this.Ox + UiChoice.MaxW * 0.5f - (Slider.GetRegionWidth())), 0, (this.Sw - 12) * Scale, this.Min, this.Max));
						this.Val = (float) (Math.Floor(this.Val * 16) / 16);

						if (Float.Compare(Prev, this.Val) != 0) {
							Audio.PlaySfx("menu/moving");
							this.OnUpdate();
						} 
					} else if (Input.Instance.WasPressed("scroll")) {
						float Last = this.Val;
						this.Val = MathUtils.Clamp(this.Min, this.Max, this.Val + 1f / 16f * Input.Instance.GetAmount());

						if (Float.Compare(Last, this.Val) != 0) {
							Audio.PlaySfx("menu/moving");
							OnUpdate();
						} 
					} 
				} 
			} 
		}

		public Void OnUpdate() {

		}

		protected override bool CheckHover() {
			return CollisionHelper.Check((int) (Input.Instance.UiMouse.X + InGameState.SettingsX), (int) Input.Instance.UiMouse.Y, (int) (this.Ox - (UiChoice.MaxW * 0.5f)), (int) (this.Y - 4 + H * 0.5f), (int) (UiChoice.MaxW * Scale), 10);
		}
	}
}
