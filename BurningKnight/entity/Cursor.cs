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
		}

		public override void Render() {
			Graphics.Batch.FillRectangle(new RectangleF(X - 1, Y - 1, 2, 2), Color.Red);
		}
	}
}