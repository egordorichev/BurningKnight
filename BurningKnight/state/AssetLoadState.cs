using System;
using BurningKnight.assets;
using Lens;
using Lens.assets;
using Lens.game;
using Lens.graphics;
using Microsoft.Xna.Framework;

namespace BurningKnight.state {
	public class AssetLoadState : GameState {
		private TextureRegion logo;
		private TextureRegion egor;
		private TextureRegion jose;
		private TextureRegion mate;
		private float t;
		private Color subColor = new Color(0.6f, 0.6f, 0.6f, 1f);

		public override void Init() {
			base.Init();

			logo = new TextureRegion(Textures.FastLoad("Content/logo.png"));
			egor = new TextureRegion(Textures.FastLoad("Content/egor.png"));
			jose = new TextureRegion(Textures.FastLoad("Content/jose.png"));
			mate = new TextureRegion(Textures.FastLoad("Content/mate.png"));
		}

		public override void Update(float dt) {
			base.Update(dt);
			t += dt;
		}

		public override void RenderUi() {
			base.RenderUi();
			
			var scale = 0.6f;
			var origin = egor.Center;
			var av = 0; // 0.04f;
			var u = 225;
			var v = 235;

			Graphics.Render(egor, new Vector2(80, 135), (float) Math.Cos(t * 0.3f) * av, origin, new Vector2(scale));
			Print("Egor Dorichev", 80, u);
			Graphics.Color = subColor;
			Print("Code, Art, Sfx & Design", 80, v); 
			Graphics.Color = ColorUtils.WhiteColor;
			
			Graphics.Render(jose, new Vector2(240, 135), (float) Math.Cos(0.2f + t * 0.33f) * av, origin, new Vector2(scale));
			Print("Jose Ramon", 240, u);
			Graphics.Color = subColor;
			Print("Music & Sfx", 240, v);
			Graphics.Color = ColorUtils.WhiteColor;

			Graphics.Render(mate, new Vector2(400, 135), (float) Math.Cos(0.5f + t * 0.24f) * av, origin, new Vector2(scale));
			Print("Mate Cziner", 400, u);
			Graphics.Color = subColor;
			Print("Art & Design", 400, v);
			Graphics.Color = ColorUtils.WhiteColor;
		}

		private void Print(string s, int x, int y) {
			Graphics.Print(s, Font.Small, (int) (x - Font.Small.MeasureString(s).Width / 2f), y);
		}
	}
}