using BurningKnight.assets;
using BurningKnight.ui.str;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.util.camera;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Timer = Lens.util.timer.Timer;

namespace BurningKnight.ui.dialog {
	public class UiDialog : FrameRenderer {
		public Entity Owner;

		private TextureRegion triangle;
		private UiString str;

		private bool saying;
		
		public override void Init() {
			base.Init();
			
			Width = 172;
			Height = 4;
				
			Setup("ui", "dialog_");
			triangle = CommonAse.Ui.GetSlice("dialog_tri");
			
			str = new UiString(Font.Small);
			Area.Add(str);

			str.Paused = true;
			str.Depth = Depth + 1;
			str.WidthLimit = (int) (Width - 16);
			
			str.FinishedTyping += s => {
				Timer.Add(() => {
					saying = false;
					
					Tween.To(0, 255, x => Tint.A = (byte) x, 0.3f).OnEnd = () => {
						str.Width = 4;
						str.Height = 4;
					};
				}, 0.5f);
			};
			
			Tint.A = 0;
		}

		public override void Destroy() {
			base.Destroy();
			str.Done = true;
		}

		public void Say(string s) {
			if (!saying) {
				Tween.To(255, 0, x => Tint.A = (byte) x, 0.3f);
			}
			
			saying = true;
			str.Label = Locale.Get(s);
		}

		public override void RenderFrame() {
			if (Tint.A == 0) {
				return;
			}
			
			base.RenderFrame();
			
			Graphics.Color = Tint;
			Graphics.Render(triangle, Position + new Vector2((Width - triangle.Width) / 2f, Height - 1f));
			Graphics.Color = ColorUtils.WhiteColor;
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			Position = Camera.Instance.CameraToUi(new Vector2(Owner.CenterX, Owner.Y - 4));
			Height = str.Height + 12;
			X -= Width / 2;
			Y -= Height;

			str.Tint = Tint;
			str.Position = Position + new Vector2(8, 4);
		}
	}
}