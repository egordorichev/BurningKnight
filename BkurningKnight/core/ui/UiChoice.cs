using BurningKnight.core.assets;
using BurningKnight.core.entity;
using BurningKnight.core.game.input;
using BurningKnight.core.game.state;
using BurningKnight.core.util;

namespace BurningKnight.core.ui {
	public class UiChoice : UiButton {
		private string[] Choices;
		private int Current;
		private string Def;
		public static float MaxW;
		private int[] Sizes;

		public UiChoice(string Label, int X, int Y) {
			base(Label, X, Y);
			this.Def = this.Label;
		}

		public UiChoice SetChoices(string Choices) {
			this.Choices = Choices;
			Sizes = new int[Choices.Length];
			int MaxLen = 0;
			int StartLen = 0;

			for (int I = 0; I < this.Choices.Length; I++) {
				if (Locale.Has(this.Choices[I])) {
					this.Choices[I] = Locale.Get(this.Choices[I]);
				} 

				Graphics.Layout.SetText(Graphics.Medium, this.Choices[I]);
				this.Sizes[I] = (int) Graphics.Layout.Width;

				if (I == 0) {
					StartLen = this.Sizes[0];
				} 

				if (Sizes[I] > MaxLen) {
					MaxLen = Sizes[I];
				} 
			}

			if (Current < 0) {
				Current += this.Choices.Length;
			} 

			this.Current = 0;
			this.SetLabel(this.Def + ": " + this.Choices[this.Current]);
			float W = this.W - StartLen + MaxLen;
			MaxW = Math.Max(W, MaxW);

			return this;
		}

		public override Void OnClick() {
			base.OnClick();
			this.SetCurrent((this.Current + (Input.Instance.IsDown("Mouse1") ? -1 : 1)) % this.Choices.Length);
		}

		protected override bool CheckHover() {
			return CollisionHelper.Check((int) (Input.Instance.UiMouse.X + InGameState.SettingsX), (int) Input.Instance.UiMouse.Y, (int) (this.X - MaxW / 2 * 1.2f), (int) (this.Y - this.H / 2 + 3), (int) (MaxW * 1.2f), this.H);
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
			Graphics.Medium.Draw(Graphics.Batch, this.Def, 2, 16);
			SetColor();
			Graphics.Medium.Draw(Graphics.Batch, this.Choices[this.Current], MaxW + 4 - this.Sizes[this.Current], 16);
			Graphics.Medium.SetColor(1, 1, 1, 1);
			Graphics.Batch.End();
			Graphics.Text.End();
			Graphics.Shadows.Begin();
			Graphics.Batch.Begin();
			Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
			Texture Texture = Graphics.Text.GetColorBufferTexture();
			Texture.SetFilter(Texture.TextureFilter.Nearest, Texture.TextureFilter.Nearest);
			Graphics.Batch.Draw(Texture, this.X - MaxW / 2 + 2, this.Y - this.H / 2, MaxW / 2 + 4, this.H / 2 + 6, MaxW + 5, this.H + 16, this.Scale, this.Scale, (float) (Math.Cos(this.Y / 12 + Dungeon.Time * 6) * (this.Mx / MaxW * 16 + 1f)), 0, 0, (int) (MaxW + 5), this.H + 16, false, true);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}

		protected Void SetColor() {
			Graphics.Medium.SetColor(0.6f, 0.6f, 0.6f, 1);
		}

		public UiChoice SetCurrent(int Current) {
			if (Current < 0) {
				Current += this.Choices.Length;
			} 

			this.Current = Current;
			this.SetLabel(this.Def + ": " + this.Choices[this.Current]);
			this.OnUpdate();

			return this;
		}

		public int GetCurrent() {
			return this.Current;
		}

		public Void OnUpdate() {

		}
	}
}
