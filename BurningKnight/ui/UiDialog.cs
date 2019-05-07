using BurningKnight.assets;
using Lens.entity;
using Lens.graphics;
using Lens.util.camera;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class UiDialog : FrameRenderer {
		public Entity Owner;

		private TextureRegion triangle;
		
		public override void Init() {
			base.Init();
			
			Width = 32;
			Height = 32;
				
			Setup("ui", "dialog_");
			triangle = CommonAse.Ui.GetSlice("dialog_tri");
		}

		public override void RenderFrame() {
			base.RenderFrame();
			Graphics.Render(triangle, Position + new Vector2((Width - triangle.Width) / 2f, Height - 1f));
		}

		public override void Update(float dt) {
			base.Update(dt);
			
			Position = Camera.Instance.CameraToUi(new Vector2(Owner.CenterX, Owner.Y - 4));
			X -= Width / 2;
			Y -= Height;
		}
	}
}