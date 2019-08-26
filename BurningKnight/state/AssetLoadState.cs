using System;
using Lens;
using Lens.assets;
using Lens.game;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.state {
	public class AssetLoadState : GameState {
		private TextureRegion logo;
		private TextureRegion egor;
		private float t;

		public override void Init() {
			base.Init();

			logo = new TextureRegion(Textures.FastLoad("Content/logo.png"));
			egor = new TextureRegion(Textures.FastLoad("Content/egor.png"));
		}

		public override void Update(float dt) {
			base.Update(dt);
			t += dt;
		}


		public override void RenderUi() {
			base.RenderUi();

			var origin = egor.Center;
			var scale = Math.Min(Display.UiWidth / (egor.Width * 3 + 40), Display.UiHeight / (egor.Height + 20));

			Graphics.Render(egor, new Vector2(10) + origin, (float) Math.Cos(t * 0.3f) * 10f, origin, new Vector2(scale));
		}
	}
}