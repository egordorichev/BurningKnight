using BurningKnight.entity;
using BurningKnight.game.input;
using BurningKnight.game.state;
using BurningKnight.util;

namespace BurningKnight.ui {
	public class UiKey : UiButton {
		private string KeyId;
		private string SecondLabel;
		private int SecondW;

		public UiKey(string Label, int X, int Y) {
			base(Label, X, Y);
			KeyId = Label;
			SetSecond();
			W += 32f;
			UiChoice.MaxW = Math.Max(W, UiChoice.MaxW);
		}

		public override void OnClick() {
			Audio.PlaySfx("menu/change_parameter");
			Input.Listener = this;
			SetSecond();
		}

		protected void SetSecond() {
			if (Input.Listener == this) {
				SecondLabel = Locale.Get("assign_key");
			}
			else {
				SecondLabel = Input.Instance.GetBinding(KeyId);

				if (SecondLabel == null)
					SecondLabel = Locale.Get("none");
				else if (SecondLabel.Equals("Mouse0"))
					SecondLabel = "LMB";
				else if (SecondLabel.Equals("Mouse1")) SecondLabel = "RMB";
			}


			W -= SecondW;
			Graphics.Layout.SetText(Graphics.Medium, SecondLabel);
			SecondW = (int) Graphics.Layout.Width;
			W += SecondW;
		}

		public override void SetLabel(string Label) {
			base.SetLabel(Label);
		}

		public void Set(string Id) {
			Input.Listener = null;
			Audio.PlaySfx("menu/select");
			Input.Instance.Rebind(KeyId, Input.Instance.GetBinding(KeyId), Id);
			SetSecond();
		}

		public override void Render() {
			ScaleMod = 0.5f;
			Graphics.Batch.End();
			Graphics.Shadows.End();
			Graphics.Text.Begin();
			Graphics.Batch.Begin();
			Graphics.Batch.SetProjectionMatrix(Camera.Nil.Combined);
			Gdx.Gl.GlClearColor(0, 0, 0, 0);
			Gdx.Gl.GlClear(GL20.GL_COLOR_BUFFER_BIT | GL20.GL_DEPTH_BUFFER_BIT | (Gdx.Graphics.GetBufferFormat().CoverageSampling ? GL20.GL_COVERAGE_BUFFER_BIT_NV : 0));
			Graphics.Medium.SetColor(R * Ar, G * Ag, B * Ab, 1);
			Graphics.Medium.Draw(Graphics.Batch, Label, 2, 16);
			var V = 0.7f;
			Graphics.Medium.SetColor(V, V, V, 1);
			Graphics.Medium.Draw(Graphics.Batch, SecondLabel, UiChoice.MaxW - SecondW + 2, 16);
			Graphics.Medium.SetColor(1, 1, 1, 1);
			Graphics.Batch.End();
			Graphics.Text.End();
			Graphics.Shadows.Begin();
			Graphics.Batch.Begin();
			Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
			Texture Texture = Graphics.Text.GetColorBufferTexture();
			Texture.SetFilter(Texture.TextureFilter.Nearest, Texture.TextureFilter.Nearest);
			Graphics.Batch.Draw(Texture, this.X - UiChoice.MaxW / 2 + 2, this.Y - H / 2, UiChoice.MaxW / 2 + 4, H / 2 + 6, UiChoice.MaxW + 5, H + 16, Scale, Scale, Math.Cos(this.Y / 12 + Dungeon.Time * 6) * (Mx / UiChoice.MaxW * 5 + 1f), 0, 0,
				(int) (UiChoice.MaxW + 5), H + 16, false, true);
			Graphics.Batch.SetColor(1, 1, 1, 1);
		}

		protected override bool CheckHover() {
			return CollisionHelper.Check((int) (Input.Instance.UiMouse.X + State.SettingsX), (int) Input.Instance.UiMouse.Y, (int) (this.X - UiChoice.MaxW / 2), this.Y - H / 2 + 3, (int) UiChoice.MaxW, H);
		}
	}
}