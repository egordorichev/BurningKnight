using BurningKnight.entity;
using BurningKnight.game.input;
using BurningKnight.game.state;
using BurningKnight.util;

namespace BurningKnight.ui {
	public class UiChoice : UiButton {
		public static float MaxW;
		private string[] Choices;
		private int Current;
		private string Def;
		private int[] Sizes;

		public UiChoice(string Label, int X, int Y) {
			base(Label, X, Y);
			Def = this.Label;
		}

		public UiChoice SetChoices(string Choices) {
			this.Choices = Choices;
			Sizes = new int[Choices.Length];
			var MaxLen = 0;
			var StartLen = 0;

			for (var I = 0; I < this.Choices.Length; I++) {
				if (Locale.Has(this.Choices[I])) this.Choices[I] = Locale.Get(this.Choices[I]);

				Graphics.Layout.SetText(Graphics.Medium, this.Choices[I]);
				Sizes[I] = (int) Graphics.Layout.Width;

				if (I == 0) StartLen = Sizes[0];

				if (Sizes[I] > MaxLen) MaxLen = Sizes[I];
			}

			if (Current < 0) Current += this.Choices.Length;

			Current = 0;
			SetLabel(Def + ": " + this.Choices[Current]);
			float W = this.W - StartLen + MaxLen;
			MaxW = Math.Max(W, MaxW);

			return this;
		}

		public override void OnClick() {
			base.OnClick();
			SetCurrent((Current + (Input.Instance.IsDown("Mouse1") ? -1 : 1)) % Choices.Length);
		}

		protected override bool CheckHover() {
			return CollisionHelper.Check((int) (Input.Instance.UiMouse.X + State.SettingsX), (int) Input.Instance.UiMouse.Y, (int) (this.X - MaxW / 2 * 1.2f), this.Y - H / 2 + 3, (int) (MaxW * 1.2f), H);
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
			Graphics.Medium.Draw(Graphics.Batch, Def, 2, 16);
			SetColor();
			Graphics.Medium.Draw(Graphics.Batch, Choices[Current], MaxW + 4 - Sizes[Current], 16);
			Graphics.Medium.SetColor(1, 1, 1, 1);
			Graphics.Batch.End();
			Graphics.Text.End();
			Graphics.Shadows.Begin();
			Graphics.Batch.Begin();
			Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
			Texture Texture = Graphics.Text.GetColorBufferTexture();
			Texture.SetFilter(Texture.TextureFilter.Nearest, Texture.TextureFilter.Nearest);
			Graphics.Batch.Draw(Texture, this.X - MaxW / 2 + 2, this.Y - H / 2, MaxW / 2 + 4, H / 2 + 6, MaxW + 5, H + 16, Scale, Scale, Math.Cos(this.Y / 12 + Dungeon.Time * 6) * (Mx / MaxW * 16 + 1f), 0, 0, (int) (MaxW + 5), H + 16, false,
				true);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}

		protected void SetColor() {
			Graphics.Medium.SetColor(0.6f, 0.6f, 0.6f, 1);
		}

		public UiChoice SetCurrent(int Current) {
			if (Current < 0) Current += Choices.Length;

			this.Current = Current;
			SetLabel(Def + ": " + Choices[this.Current]);
			OnUpdate();

			return this;
		}

		public int GetCurrent() {
			return Current;
		}

		public void OnUpdate() {
		}
	}
}