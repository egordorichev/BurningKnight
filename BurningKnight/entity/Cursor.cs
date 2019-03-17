using BurningKnight.util;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.input;
using Lens.util;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity {
	public class Cursor : Entity {
		private TextureRegion region;
		private Vector2 scale = new Vector2(1);
		
		public override void Init() {
			base.Init();

			AlwaysActive = true;
			AlwaysVisible = true;
			Depth = Layers.Cursor;

			var anim = Animations.Get("ui");
			region = anim.GetSlice("cursor_j");
		}

		public override void Update(float dt) {
			base.Update(dt);

			Position = Input.Mouse.UiPosition;
			Position -= new Vector2(Display.UiWidth / 2f, Display.UiHeight / 2f);

			if (Input.Mouse.WasPressedLeftButton || Input.Mouse.WasPressedRightButton) {
				Tween.To(1.3f, scale.X, x => { scale.X = scale.Y = x; }, 0.05f).OnEnd = () => Tween.To(scale, new { X = 1, Y = 1 }, 0.15f);
			}
		}

		public override void Render() {
			var pos = Input.Mouse.UiPosition;
			Graphics.Render(region, pos, 0, region.Center, scale);
		}
	}
}