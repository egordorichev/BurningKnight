using BurningKnight.game.input;
using BurningKnight.util;

namespace BurningKnight.ui {
	public class UiButton : UiEntity {
		protected float Ab = 1f;
		protected float Ag = 1f;
		protected float Ar = 1f;
		public float B = 0.7f;
		private bool DisableClick;
		public float G = 0.7f;
		public int H;
		protected bool Hover;
		protected string Label;
		private Tween.Task Last;
		protected float Mx = 1f;
		protected bool PlaySfx = true;
		public float R = 0.7f;
		public float Rb = 0.7f;
		public float Rg = 0.7f;
		public float Rr = 0.7f;
		public float Scale = 1f;
		public float ScaleMod = 1f;
		public bool Sparks;
		public int W;

		public UiButton(string Label, int X, int Y) {
			SetLabel(Label);
			this.Y = Y;
			this.X = X;
		}

		public UiButton(string Label, int X, int Y, bool DisableClick) {
			SetLabel(Label);
			this.Y = Y;
			this.X = X;
			this.DisableClick = DisableClick;
		}

		public void SetLabel(string Label) {
			if (Label == null) return;

			var Old = Label;

			if (Locale.Has(Label)) Label = Locale.Get(Label);

			this.Label = Label;
			ModLabel(Old);
			Graphics.Layout.SetText(Graphics.Medium, this.Label);
			W = (int) Graphics.Layout.Width;
			H = (int) Graphics.Layout.Height;
		}

		protected void ModLabel(string Old) {
		}

		public UiButton SetSparks(bool Sparks) {
			this.Sparks = Sparks;

			return this;
		}

		public UiButton SetPlaySfx(bool PlaySfx) {
			this.PlaySfx = PlaySfx;

			return this;
		}

		public override void Update(float Dt) {
			if (Input.Instance.WasPressed("ui_accept") && !DisableClick)
				if (Hover && this.IsSelected && Input.Instance.WasPressed("ui_accept") || CheckHover() && Input.Instance.WasPressed("ui_accept")) {
					if (Last != null) Tween.Remove(Last);

					Rr = 1f;
					Rg = 1f;
					Rb = 1f;
					Last = Tween.To(new Tween.Task(1f - 0.2f * ScaleMod, 0.05f) {

		public override float GetValue() {
			return Scale;
		}

		public override void SetValue(float Value) {
			Scale = Value;
		}

		public override void OnEnd() {
			base.OnEnd();
			Last = null;
			OnClick();
			Last = Tween.To(new Tween.Task(1 + 0.2f * ScaleMod, 0.05f) {

		public override float GetValue() {
			return Scale;
		}

		public override void SetValue(float Value) {
			Scale = Value;
		}

		public override void OnEnd() {
			base.OnEnd();
			Last = null;
		}

		public override bool RunWhenPaused() {
			return true;
		}
	});
}

public override bool RunWhenPaused() {
return true;
}
});
}
}
bool H = this.Hover;
this.Hover = IsSelected || CheckHover();
if (H && !this.Hover) {
if (this.Last != null) {
Tween.Remove(this.Last);
}
this.Last = Tween.To(new Tween.Task(1f, 0.1f) {
public override float GetValue() {
return Scale;
}
public override void SetValue(float Value) {
Scale = Value;
}
public override void OnEnd() {
base.OnEnd();
Last = null;
}
public override bool RunWhenPaused() {
return true;
}
});
Tween.To(new Tween.Task(1, 0.1f) {
public override float GetValue() {
return Mx;
}
public override void SetValue(float Value) {
Mx = Value;
}
public override bool RunWhenPaused() {
return true;
}
});
OnUnhover();
this.R = 0.7f;
this.G = 0.7f;
this.B = 0.7f;
} else if ((!H && this.Hover)) {
if (this.Last != null) {
Tween.Remove(this.Last);
}
this.R = 1;
this.G = 1;
this.B = 1;
Audio.PlaySfx("menu/moving");
OnHover();
Tween.To(new Tween.Task(20, 0.1f) {
public override float GetValue() {
return Mx;
}
public override void SetValue(float Value) {
Mx = Value;
}
public override bool RunWhenPaused() {
return true;
}
});
this.Last = Tween.To(new Tween.Task(1f + 0.2f * ScaleMod, 0.1f) {
public override float GetValue() {
return Scale;
}
public override void SetValue(float Value) {
Scale = Value;
}
public override void OnEnd() {
base.OnEnd();
Last = null;
}
public override bool RunWhenPaused() {
return true;
}
});
}
float S = Dt * 4f;
this.Rr += (this.R - this.Rr) * S;
this.Rg += (this.G - this.Rg) * S;
this.Rb += (this.B - this.Rb) * S;
if (this.Hover) {
this.Area.Select(this);
}
base.Update(Dt);
}
public override void Render() {
Graphics.Batch.SetColor(this.R * this.Ar, this.G * this.Ag, this.B * this.Ab, 1);
Graphics.Batch.End();
Graphics.Shadows.End();
Graphics.Text.Begin();
Graphics.Batch.Begin();
Graphics.Batch.SetProjectionMatrix(Camera.Nil.Combined);
Gdx.Gl.GlClearColor(0, 0, 0, 0);
Gdx.Gl.GlClear(GL20.GL_COLOR_BUFFER_BIT | GL20.GL_DEPTH_BUFFER_BIT | (Gdx.Graphics.GetBufferFormat().CoverageSampling ? GL20.GL_COVERAGE_BUFFER_BIT_NV : 0));
Graphics.Medium.Draw(Graphics.Batch, this.Label, 2, 16);
Graphics.Batch.End();
Graphics.Text.End();
Graphics.Shadows.Begin();
Graphics.Batch.Begin();
Graphics.Batch.SetProjectionMatrix(Camera.Ui.Combined);
Texture Texture = Graphics.Text.GetColorBufferTexture();
Texture.SetFilter(Texture.TextureFilter.Nearest, Texture.TextureFilter.Nearest);
Graphics.Batch.Draw(Texture, this.X - this.W / 2 + 2, this.Y - this.H / 2, this.W / 2 + 4, this.H / 2 + 6, this.W + 4, this.H + 16, this.Scale, this.Scale, (float) (Math.Cos(this.Y / 12 + Dungeon.Time * 6) * (this.Mx / this.W * 16 + 1f)), 0
, 0, this.W + 4, this.H + 16, false, true);
Graphics.Batch.SetColor(1, 1, 1, 1);
}
protected bool CheckHover() {
return CollisionHelper.Check((int) (Input.Instance.UiMouse.X + InGameState.SettingsX), (int) Input.Instance.UiMouse.Y, (int) (this.X - this.W / 2), (int) (this.Y - this.H / 2 + 3), (int) (this.W), this.H);
}
public void OnClick() {
if (this.PlaySfx) {
Audio.PlaySfx("menu/select");
}
}
protected void OnUnhover() {
}
protected void OnHover() {
}
}
}