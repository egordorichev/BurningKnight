using System;
using BurningKnight.entity.editor.command;
using BurningKnight.entity.level;
using BurningKnight.entity.level.biome;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.input;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Console = BurningKnight.debug.Console;

namespace BurningKnight.entity.editor {
	public class Editor : Entity {
		public EditorCursor Cursor;
		public CommandQueue Commands;
		public Camera Camera;
		public Level Level;
		public TileSelect TileSelect;
		public bool ShowPanes = true;

		public override void Init() {
			base.Init();

			AlwaysVisible = true;
			AlwaysActive = true;
			
			Engine.Instance.State.Ui.Add(new Console(Area));

			Commands = new CommandQueue {
				Editor = this
			};
			
			Engine.Instance.State.Ui.Add(Cursor = new EditorCursor {
				Editor = this
			});
			
			Engine.Instance.State.Ui.Add(Camera = new Camera(new CameraDriver()) {
				Position = new Vector2(Display.Width / 2f, Display.Height / 2f)
			});

			Area.Add(Level = new RegularLevel(BiomeRegistry.Defined[Biome.Castle]) {
				Width = 32, Height = 32
			});

			Level.Setup();
			Level.Fill(Tile.FloorA);
			Level.TileUp();
			
			Engine.Instance.State.Ui.Add(TileSelect = new TileSelect {
				Editor = this
			});
		}

		private void OnClick(Vector2 pos) {
			if (TileSelect.OnClick(pos)) {
				return;
			}
			
			Cursor.OnClick(pos);
		}

		public override void Update(float dt) {
			base.Update(dt);

			if (Input.Keyboard.IsDown(Keys.LeftControl)) {
				if (Input.Keyboard.WasPressed(Keys.Z)) {
					Commands.Undo();
				}
				
				if (Input.Keyboard.WasPressed(Keys.Y)) {
					Commands.Redo();
				}
			}

			if (Input.Mouse.WasPressedLeftButton || (Input.Mouse.CheckLeftButton && Input.Mouse.WasMoved)) {
				OnClick(Input.Mouse.GamePosition);
			}
		}

		public override void Render() {
			base.Render();

			if (Cursor.CurrentMode != EditorCursor.Mode.Drag) {
				var x = (int) Math.Floor(Input.Mouse.GamePosition.X / 16); 
				var y = (int) Math.Floor(Input.Mouse.GamePosition.Y / 16);

				if (Level.IsInside(x, y)) {
					Graphics.Batch.DrawRectangle(new RectangleF(x * 16, y * 16, 16, 16), new Color(1, 1, 1, 0.5f));
				}
			}
		}
	}
}