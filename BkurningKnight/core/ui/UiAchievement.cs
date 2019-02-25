using BurningKnight.core.assets;
using BurningKnight.core.entity;
using BurningKnight.core.entity.item;
using BurningKnight.core.util;

namespace BurningKnight.core.ui {
	public class UiAchievement : Entity {
		protected void _Init() {
			{
				AlwaysActive = true;
				AlwaysRender = true;
				Depth = 32;
			}
		}

		public string Text;
		public string Extra;
		public bool Unlock;
		private float W2;
		private static TextureRegion Left = Graphics.GetTexture("ui-achievement_left");
		private static TextureRegion Center = Graphics.GetTexture("ui-achievement_center");
		private static TextureRegion Right = Graphics.GetTexture("ui-achievement_right");
		private static TextureRegion Ach = Graphics.GetTexture("ui-ach");
		public TextureRegion Icon;

		public override Void Init() {
			base.Init();
			this.W = 0;
			Graphics.Layout.SetText(this.Extra != null ? Graphics.Medium : Graphics.Small, this.Text);
			float W1 = Graphics.Layout.Width;

			if (this.Extra != null) {
				Graphics.Layout.SetText(Graphics.Small, this.Extra);
				this.W2 = Graphics.Layout.Width;
			} 

			this.H = 38;
			this.W = Math.Max(this.W2, W1) + (Icon == Item.Missing ? 3 : 32) + 9 + 5;
			this.Y = -this.H * 3;
			this.X = Display.UI_WIDTH - 2 - this.W;
			Tween.To(new Tween.Task(2, 0.5f, Tween.Type.BACK_OUT) {
				public override float GetValue() {
					return Y;
				}

				public override Void SetValue(float Value) {
					Y = Value;
				}

				public override bool RunWhenPaused() {
					return true;
				}

				public override Void OnEnd() {
					Tween.To(new Tween.Task(-H, 0.5f, Tween.Type.BACK_OUT) {
						public override float GetValue() {
							return Y;
						}

						public override Void SetValue(float Value) {
							Y = Value;
						}

						public override bool RunWhenPaused() {
							return true;
						}

						public override Void OnEnd() {
							SetDone(true);
						}
					}).Delay(10);
				}
			});
		}

		public override Void Render() {
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
