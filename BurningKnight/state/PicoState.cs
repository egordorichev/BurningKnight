using BurningKnight.entity;
using BurningKnight.util;
using Lens;
using Lens.assets;
using Lens.game;
using Lens.graphics;
using Lens.util.camera;
using Microsoft.Xna.Framework;

namespace BurningKnight.state {
	public class PicoState : GameState {
		private TextureRegion frame;
		private TextureRegion fill;

		public override void Init() {
			base.Init();

			var anim = Animations.Get("monitor");

			frame = anim.GetSlice("frame");
			fill = anim.GetSlice("fill");

			var c = new Camera(new FollowingDriver());
			
			Ui.Add(c);
			Ui.Add(new Cursor());
			
			c.Position = new Vector2(Display.Width / 2f, Display.Height / 2f);
		}

		public override void Render() {
			base.Render();
			
			Graphics.Render(frame, new Vector2(32, 10));
			Graphics.Render(fill, new Vector2(37, 26));
		}
	}
}