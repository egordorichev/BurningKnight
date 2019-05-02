using BurningKnight.assets;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.input;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity {
	public class Cursor : Entity {
		protected TextureRegion Region;
		private Vector2 scale = new Vector2(1);
		private bool first;
		
		public override void Init() {
			base.Init();

			AlwaysActive = true;
			AlwaysVisible = true;
			Depth = Layers.Cursor;

			Region = CommonAse.Ui.GetSlice("cursor_j");
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Input.Mouse.WasMoved || !first) {
				first = true;
				Position = Input.Mouse.UiPosition;
				Position -= new Vector2(Display.UiWidth / 2f, Display.UiHeight / 2f);
			}

			if (Input.Mouse.WasPressedLeftButton || Input.Mouse.WasPressedRightButton) {
				Tween.To(1.3f, scale.X, x => { scale.X = scale.Y = x; }, 0.05f).OnEnd = () =>
					Tween.To(1f, scale.X, x => { scale.X = scale.Y = x; }, 0.15f);
			}
		}

		public override void Render() {
			if (Settings.HideCursor) {
				return;
			}
			
			Graphics.Render(Region, Input.Mouse.UiPosition, 0, Region.Center, scale);
		}
	}
}