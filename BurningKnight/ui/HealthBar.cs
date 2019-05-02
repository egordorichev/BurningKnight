using BurningKnight.assets;
using Lens;
using Lens.entity;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.ui {
	public class HealthBar : Entity {
		private TextureRegion frame;
		private TextureRegion fill;
		
		private Entity entity;
		
		public HealthBar(Entity owner) {
			entity = owner;
		}

		public override void AddComponents() {
			base.AddComponents();

			AlwaysActive = true;
			AlwaysVisible = true;

			frame = CommonAse.Ui.GetSlice("hb_frame");
			fill = CommonAse.Ui.GetSlice("hb");
			
			Width = frame.Width;
			Height = frame.Height;

			CenterX = Display.UiWidth / 2f;
			Y = 3;
		}

		public override void Render() {
			Graphics.Render(frame, Position);
			Graphics.Render(fill, Position + new Vector2(1));
		}
	}
}