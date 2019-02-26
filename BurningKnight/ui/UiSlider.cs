using BurningKnight.entity;
using BurningKnight.game.input;
using BurningKnight.game.state;
using BurningKnight.util;

namespace BurningKnight.ui {
	public class UiSlider : UiButton {
		private static TextureRegion Slider = Graphics.GetTexture("ui-slider");
		private static TextureRegion Fill = Graphics.GetTexture("ui-slider_fill");
		private float Max;
		private float Min;
		private float Ox;
		private float Sw;
		protected float Val;

		public UiSlider(string Label, int X, int Y) {
			base(Label, X, Y, true);
			Ox = X;
			Min = 0;
			Max = 1;
			Val = 1;
		}

		public UiSlider SetValue(float Val) {
			this.Val = Val;

			return this;
		}

		public override void SetLabel(string Label) {
			base.SetLabel(Label);
			this.X += W / 2;
			Sw = Slider.GetRegionWidth() + 8;
			W += Sw;
			UiChoice.MaxW = Math.Max(W, UiChoice.MaxW);
		}

		public override void Render() {
			Graphics.Batch.End();
			Graphics.Shadows.End();
			Graphics.Text.Begin();
			Graphics.Batch.Begin();
			Graphics.Batch.SetProjectionMatrix(Camera.Nil.Combined);
			Gdx.Gl.GlClearColor(0, 0, 0, 0);
			Gdx.Gl.GlClear(GL20.GL_COLOR_BUFFER_BIT | GL20.GL_DEPTH_BUFFER_BIT | (Gdx.Graphics.GetBufferFormat().CoverageSampling ? GL20.GL_COVERAGE_BUFFER_BIT_NV : 0));
			Graphics.Medium.SetColor(R * Ar, G * Ag, B * Ab, 1);
			Graphics.Medium.Draw(Graphics.Batch, Label, 2, 16);
			var W = UiChoice.MaxW;
			Graphics.Medium.Draw(Graphics.Batch, Label, Ox - W * 0.5f + 4, this.Y - H / 2 + 16);
			Graphics.Medium.SetColor(1, 1, 1, 1);
			W = Slider.GetRegionWidth() - 4;
			var V = MathUtils.Map(Val, Min, Max, 0, W / 4);
			var Vl = 0.2f;
			Graphics.Batch.SetColor(Vl, Vl, Vl, 1);
			Graphics.Render(Fill, UiChoice.MaxW - W + 2, H * 0.5f + 2, 0, 0, 0, false, false, W / 4 * Scale, Scale * 1.1f);
			Graphics.Batch.SetColor(1, 1, 1, 1);
			Graphics.Render(Fill, UiChoice.MaxW - W + 2, H * 0.5f + 2, 0, 0, 0, false, false, V * Scale, Scale * 1.1f);
			Graphics.Render(Slider, UiChoice.MaxW - W, H * 0.5f, 0, 0, 0, false, false, Scale, Scale);
			var S = (int) Math.Floor(Val * 100f) + "%";
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
			Graphics.Batch.Draw(Texture, this.X - UiChoice.MaxW / 2 + 2, this.Y - H / 2, this.W / 2, H / 2 + 8, this.W + 72, H + 16, Scale, Scale, Math.Cos(this.Y / 12 + Dungeon.Time * 6) * (Mx / UiChoice.MaxW * 10 + 1f), 0, 0, this.W + 72,
				H + 16, false, true);
		}

		public override void Update(float Dt) {
			base.Update(Dt);

			if (this.IsSelected || Hover) {
				if (Input.Instance.WasPressed("ui_left")) {
					Val = MathUtils.Clamp(Min, Max, Val - (Max - Min) / 16);
					Audio.PlaySfx("menu/moving");
					OnUpdate();
				}
				else if (Input.Instance.WasPressed("ui_right")) {
					Val = MathUtils.Clamp(Min, Max, Val + (Max - Min) / 16);
					Audio.PlaySfx("menu/moving");
					OnUpdate();
				}

				if (CheckHover()) {
					if (Input.Instance.IsDown("mouse")) {
						var Prev = Val;
						Val = MathUtils.Clamp(Min, Max, MathUtils.Map(Input.Instance.UiMouse.X + State.SettingsX - (Ox + UiChoice.MaxW * 0.5f - Slider.GetRegionWidth()), 0, (Sw - 12) * Scale, Min, Max));
						Val = Math.Floor(Val * 16) / 16;

						if (Float.Compare(Prev, Val) != 0) {
							Audio.PlaySfx("menu/moving");
							OnUpdate();
						}
					}
					else if (Input.Instance.WasPressed("scroll")) {
						var Last = Val;
						Val = MathUtils.Clamp(Min, Max, Val + 1f / 16f * Input.Instance.GetAmount());

						if (Float.Compare(Last, Val) != 0) {
							Audio.PlaySfx("menu/moving");
							OnUpdate();
						}
					}
				}
			}
		}

		public void OnUpdate() {
		}

		protected override bool CheckHover() {
			return CollisionHelper.Check((int) (Input.Instance.UiMouse.X + State.SettingsX), (int) Input.Instance.UiMouse.Y, (int) (Ox - UiChoice.MaxW * 0.5f), (int) (this.Y - 4 + H * 0.5f), (int) (UiChoice.MaxW * Scale), 10);
		}
	}
}