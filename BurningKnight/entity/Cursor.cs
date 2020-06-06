using BurningKnight.assets;
using BurningKnight.entity.component;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.graphics;
using Lens.input;
using Lens.util.tween;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity {
	public class Cursor : Entity {
		private Vector2 scale = new Vector2(1);
		private bool first;

		private TextureRegion[] regions;

		public override void Init() {
			base.Init();

			AlwaysActive = true;
			AlwaysVisible = true;
			Depth = Layers.Cursor;

			regions = new[] {
				CommonAse.Ui.GetSlice("cursor_a"),
				CommonAse.Ui.GetSlice("cursor_b"),
				CommonAse.Ui.GetSlice("cursor_c"),
				CommonAse.Ui.GetSlice("cursor_d"),
				CommonAse.Ui.GetSlice("cursor_e"),
				CommonAse.Ui.GetSlice("cursor_f"),
				CommonAse.Ui.GetSlice("cursor_g"),
				CommonAse.Ui.GetSlice("cursor_j"),
				CommonAse.Ui.GetSlice("cursor_k")
			};
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

			var r = regions[Settings.Cursor];
			var pos = Input.Mouse.UiPosition;
			
			Graphics.Render(r, pos, 0, r.Center, scale);
		}
	}
}