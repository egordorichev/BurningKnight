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
		private Vector2 scale = new Vector2(2f, 1);
		private float a = 1;
		
		public override void Init() {
			base.Init();

			AlwaysActive = true;
			AlwaysVisible = true;
			Depth = 32;

			var anim = Animations.Get("ui");
			region = anim.GetSlice("cursor_j");
		}

		public override void Update(float dt) {
			base.Update(dt);


			Position = Input.Mouse.UiPosition;
			Position.X -= Display.UiWidth / 2f;
			Position.Y -= Display.UiHeight / 2f;

			if (Input.Mouse.WasPressedLeftButton) {
				Tween.To(this, new { a = 2.3f }, 1.1f).OnEnd = () => Tween.To(this, new { a = 1 }, 0.2f);
			}
		}

		public override void Render() {
			var pos = Input.Mouse.UiPosition;
			Graphics.Render(region, pos, 0, region.Center, new Vector2(a));
		}
	}
}