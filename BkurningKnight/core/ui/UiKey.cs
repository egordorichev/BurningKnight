using BurningKnight.core.assets;
using BurningKnight.core.entity;
using BurningKnight.core.game.input;
using BurningKnight.core.game.state;
using BurningKnight.core.util;

namespace BurningKnight.core.ui {
	public class UiKey : UiButton {
		private string KeyId;
		private string SecondLabel;
		private int SecondW;

		public UiKey(string Label, int X, int Y) {
			base(Label, X, Y);
			this.KeyId = Label;
			SetSecond();
			W += 32f;
			UiChoice.MaxW = Math.Max(W, UiChoice.MaxW);
		}

		public override Void OnClick() {
			Audio.PlaySfx("menu/change_parameter");
			Input.Listener = this;
			SetSecond();
		}

		protected Void SetSecond() {
			if (Input.Listener == this) {
				this.SecondLabel = Locale.Get("assign_key");
			} else {
				this.SecondLabel = Input.Instance.GetBinding(this.KeyId);

				if (this.SecondLabel == null) {
					this.SecondLabel = Locale.Get("none");
				} else if (this.SecondLabel.Equals("Mouse0")) {
					this.SecondLabel = "LMB";
				} else if (this.SecondLabel.Equals("Mouse1")) {
					this.SecondLabel = "RMB";
				} 
			}


			W -= SecondW;
			Graphics.Layout.SetText(Graphics.Medium, this.SecondLabel);
			this.SecondW = (int) Graphics.Layout.Width;
			W += SecondW;
		}

		public override Void SetLabel(string Label) {
			base.SetLabel(Label);
		}

		public Void Set(string Id) {
			Input.Listener = null;
			Audio.PlaySfx("menu/select");
			Input.Instance.Rebind(this.KeyId, Input.Instance.GetBinding(this.KeyId), Id);
			SetSecond();
		}

		public override Void Render() {
			ScaleMod = 0.5f;
			Graphics.Batch.End();
			Graphics.Shadows.End();
			Graphics.Text.Begin();
			Graphics.Batch.Begin();
			Graphics.Batch.SetProjectionMatrix(Camera.Nil.Combined);
			Gdx.Gl.GlClearColor(0, 0, 0, 0);
			Gdx.Gl.GlClear(GL20.GL_COLOR_BUFFER_BIT | GL20.GL_DEPTH_BUFFER_BIT | (Gdx.Graphics.GetBufferFormat().CoverageSampling ? GL20.GL_COVERAGE_BUFFER_BIT_NV : 0));
			Graphics.Medium.SetColor(this.R * this.Ar, this.G * this.Ag, this.B * this.Ab, 1);
			Graphics.Medium.Draw(Graphics.Batch, this.Label, 2, 16);
			float V = 0.7f;
			Graphics.Medium.SetColor(V, V, V, 1);
			Graphics.Medium.Draw(Graphics.Batch, this.SecondLabel, UiChoice.MaxW - this.SecondW + 2, 16);
			Graphics.Medium.SetColor(1, 1, 1, 1);
			Graphics.Batch.End();
			Graphics.Text.End();
			Graphics.Shadows.Begin();
			Graphics.Batch.Begin();
			Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
			Texture Texture = Graphics.Text.GetColorBufferTexture();
			Texture.SetFilter(Texture.TextureFilter.Nearest, Texture.TextureFilter.Nearest);
			Graphics.Batch.Draw(Texture, this.X - UiChoice.MaxW / 2 + 2, this.Y - this.H / 2, UiChoice.MaxW / 2 + 4, this.H / 2 + 6, UiChoice.MaxW + 5, this.H + 16, this.Scale, this.Scale, (float) (Math.Cos(this.Y / 12 + Dungeon.Time * 6) * (this.Mx / UiChoice.MaxW * 5 + 1f)), 0, 0, (int) (UiChoice.MaxW + 5), this.H + 16, false, true);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}

		protected override bool CheckHover() {
			return CollisionHelper.Check((int) (Input.Instance.UiMouse.X + InGameState.SettingsX), (int) Input.Instance.UiMouse.Y, (int) (this.X - UiChoice.MaxW / 2), (int) (this.Y - this.H / 2 + 3), (int) (UiChoice.MaxW), this.H);
		}
	}
}
