using BurningKnight.assets;
using Lens.graphics;
using Lens.input;
using Lens.util.camera;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.entity.editor {
	public class EditorCursor : Cursor {
		public enum Mode {
			Normal,
			Drag,
			Fill,
			Select
		}
		
		private TextureRegion hand;
		private TextureRegion normal;

		public Mode CurrentMode = Mode.Normal;
		public bool Drag;
		
		public override void Init() {
			base.Init();

			hand = CommonAse.Ui.GetSlice("editor_drag");
			normal = CommonAse.Ui.GetSlice("editor_default");
		}

		public override void Update(float dt) {
			base.Update(dt);

			Drag = Input.Keyboard.IsDown(Keys.Space);

			if (Drag && Input.Mouse.CheckLeftButton && Input.Mouse.WasMoved) {
				// fixme: delta doesnt count screen upscale
				Camera.Instance.Position -= Input.Mouse.PositionDelta;
			}
		}

		public override void Render() {			
			Region = CurrentMode == Mode.Drag || Drag ? hand : normal;
			base.Render();
		}
	}
}