using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.input;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace BurningKnight.entity {
	public class Cursor : Entity {
		public override void Init() {
			base.Init();

			AlwaysActive = true;
			AlwaysVisible = true;
			Depth = 32;
		}

		public override void Update(float dt) {
			base.Update(dt);

			Position = Input.Mouse.UiPosition;
			Position.X -= Display.UiWidth / 2f;
			Position.Y -= Display.UiHeight / 2f;
		}

		public override void Render() {
			var pos = Input.Mouse.UiPosition;
			Graphics.Batch.FillRectangle(new RectangleF(pos.X - 1, pos.Y - 1, 2, 2), Color.Red);
		}
	}
}