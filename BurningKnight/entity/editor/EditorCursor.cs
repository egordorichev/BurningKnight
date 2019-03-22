using System;
using BurningKnight.assets;
using BurningKnight.entity.editor.command;
using BurningKnight.entity.level;
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
		public Editor Editor;
		
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

			if (Input.Mouse.WasPressedLeftButton || (Input.Mouse.CheckLeftButton && Input.Mouse.WasMoved)) {
				if (CurrentMode == Mode.Normal) {
					var tile = Editor.TileSelect.Current;
					var x = (int) Math.Floor(Input.Mouse.GamePosition.X / 16);
					var y = (int) Math.Floor(Input.Mouse.GamePosition.Y / 16);
					
					if (Editor.Level.IsInside(x, y) && Editor.Level.Get(x, y, tile.Matches(TileFlags.LiquidLayer)) != tile) {
						Editor.Commands.Do(new SetCommand {
							X = x,
							Y = y,
							Tile = tile
						});
					}
				}
			}
		}

		public override void Render() {			
			Region = CurrentMode == Mode.Drag || Drag ? hand : normal;
			base.Render();
		}
	}
}