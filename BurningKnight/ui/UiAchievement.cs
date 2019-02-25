using BurningKnight.entity;
using BurningKnight.entity.item;
using BurningKnight.util;

namespace BurningKnight.ui {
	public class UiAchievement : Entity {
		private static TextureRegion Left = Graphics.GetTexture("ui-achievement_left");
		private static TextureRegion Center = Graphics.GetTexture("ui-achievement_center");
		private static TextureRegion Right = Graphics.GetTexture("ui-achievement_right");
		private static TextureRegion Ach = Graphics.GetTexture("ui-ach");
		public string Extra;
		public TextureRegion Icon;

		public string Text;
		public bool Unlock;
		private float W2;

		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
				Depth = 32;
			}
		}

		public override void Init() {
			base.Init();
			W = 0;
			Graphics.Layout.SetText(Extra != null ? Graphics.Medium : Graphics.Small, Text);
			float W1 = Graphics.Layout.Width;

			if (Extra != null) {
				Graphics.Layout.SetText(Graphics.Small, Extra);
				W2 = Graphics.Layout.Width;
			}

			H = 38;
			W = Math.Max(W2, W1) + (Icon == Item.Missing ? 3 : 32) + 9 + 5;
			this.Y = -H * 3;
			this.X = Display.UI_WIDTH - 2 - W;
			Tween.To(new Tween.Task(2, 0.5f, Tween.Type.BACK_OUT) {

		public override float GetValue() {
			return Y;
		}

		public override void SetValue(float Value) {
			Y = Value;
		}

		public override bool RunWhenPaused() {
			return true;
		}

		public override void OnEnd() {
			Tween.To(new Tween.Task(-H, 0.5f, Tween.Type.BACK_OUT) {

		public override float GetValue() {
			return Y;
		}

		public override void SetValue(float Value) {
			Y = Value;
		}

		public override bool RunWhenPaused() {
			return true;
		}

		public override void OnEnd() {
			SetDone(true);
		}
	}).Delay(10);
}

});
}
public override void Render() {
if (Ui.HideUi) {
return;
}
bool Missing = Icon == Item.Missing;
Graphics.Batch.SetColor(1, 1, 1, 1);
if (Unlock) {
Graphics.StartShape();
Graphics.Shape.SetColor(0.3f, 0.3f, 0.3f, 1f);
Graphics.Shape.Rect(this.X + 3, this.Y + 3, 32, 32);
Graphics.EndShape();
Graphics.Render(Icon, this.X + 3 + (32 - Icon.GetRegionWidth()) / 2, this.Y + 3 + (32 - Icon.GetRegionHeight()) / 2);
} else {
if (!Missing) {
Graphics.StartShape();
Graphics.Shape.SetColor(0, 0, 0, 1);
Graphics.Shape.Rect(this.X + 3, this.Y + 3, 32, 32);
Graphics.EndShape();
Graphics.Render(Icon, this.X + 3, this.Y + 3);
}
}
Graphics.Render(Missing ? Ach : Left, this.X, this.Y);
Graphics.Render(Center, this.X + (Missing ? 3 : 38), this.Y, 0, 0, 0, false, false, (this.W - (Missing ? 21 : 57)), 1);
Graphics.Render(Right, this.X + this.W - 19, this.Y);
float M = Missing ? 0 : 32;
if (this.Extra != null) {
Graphics.Medium.Draw(Graphics.Batch, this.Text, this.X + M + 6 + 3, this.Y + this.H - 4 - 4 - 2);
Graphics.Small.Draw(Graphics.Batch, this.Extra, this.X + M + 6 + 3, this.Y + this.H - 3 - 16 - 4);
} else {
Graphics.Small.Draw(Graphics.Batch, this.Text, this.X + M + 6 + 3, this.Y + this.H - 4 - 4 - 2 - 6);
}
}
public UiAchievement() {
_Init();
}
}
}