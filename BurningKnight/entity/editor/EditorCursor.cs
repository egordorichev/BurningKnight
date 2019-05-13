using System;
using BurningKnight.assets;
using BurningKnight.entity.editor.command;
using BurningKnight.level.tile;
using Lens.entity;
using Lens.graphics;
using Lens.input;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace BurningKnight.entity.editor {
	public class EditorCursor : Cursor {
		public enum Mode {
			Normal,
			Drag,
			Fill,
			Entity,
			Select
		}
		
		private TextureRegion hand;
		private TextureRegion normal;
		private TextureRegion fill;
		private Entity entity;
		private string entityType;

		public Mode CurrentMode = Mode.Entity;
		public bool Drag;
		public Editor Editor;
		
		public override void Init() {
			base.Init();

			hand = CommonAse.Ui.GetSlice("editor_drag");
			normal = CommonAse.Ui.GetSlice("editor_default");
			fill = CommonAse.Ui.GetSlice("editor_bucket");
		}

		public void SetEntity(Type type) {
			if (entity != null) {
				entity.Done = true;
			}
			
			entityType = type.FullName;
			entity = (Entity) Activator.CreateInstance(type);

			Editor.Area.Add(entity);
		}

		public override void Update(float dt) {
			base.Update(dt);

			Drag = Input.Keyboard.IsDown(Keys.Space);

			if (Drag && Input.Mouse.CheckLeftButton && Input.Mouse.WasMoved) {
				// fixme: delta doesnt count screen upscale
				Camera.Instance.Position -= Input.Mouse.PositionDelta;
			}

			if (Input.Keyboard.WasPressed(Keys.F) || Input.Keyboard.WasPressed(Keys.G)) {
				CurrentMode = Mode.Fill;
			}
			
			if (Input.Keyboard.WasPressed(Keys.N) || Input.Keyboard.WasPressed(Keys.B)) {
				CurrentMode = Mode.Normal;
			}
			
			if (Input.Keyboard.WasPressed(Keys.E)) {
				CurrentMode = Mode.Entity;
			}
			
			if (CurrentMode == Mode.Entity) {
				var mouse = Input.Mouse.GamePosition;

				if (Editor.EntitySelectWindow.SnapToGrid) {
					mouse.X = (float) (Math.Floor(mouse.X / 16) * 16);
					mouse.Y = (float) (Math.Floor(mouse.Y / 16) * 16);
				}
				
				if (Editor.EntitySelectWindow.Center) {
					entity.Center = mouse;
				} else {
					entity.Position = mouse;
				}
			}
		}

		public void OnClick(Vector2 pos) {
			if (Drag) {
				return;
			}
			
			var tile = Editor.TileSelect.Current;
			var x = (int) Math.Floor(pos.X / 16);
			var y = (int) Math.Floor(pos.Y / 16);

			// fixme: check if imgui handled the click
			if (!Editor.Level.IsInside(x, y)) {
				return;
			}
			
			if (CurrentMode == Mode.Entity) {
				entity = (Entity) Activator.CreateInstance(Type.GetType(entityType, true, false));
				return;
			}
			
			if (CurrentMode == Mode.Normal) {
				if (Editor.Level.Get(x, y, tile.Matches(TileFlags.LiquidLayer)) != tile) {
					Editor.Commands.Do(new SetCommand {
						X = x,
						Y = y,
						Tile = tile
					});
				}
			} else if (CurrentMode == Mode.Fill) {
				if (Editor.Level.Get(x, y, tile.Matches(TileFlags.LiquidLayer)) != tile) {
					Editor.Commands.Do(new FillCommand {
						X = x,
						Y = y,
						Tile = tile
					});
				}
			}
		}

		public override void Render() {
			if (CurrentMode == Mode.Entity) {
				return;
			}
			
			Region = CurrentMode == Mode.Drag || Drag ? hand : (CurrentMode == Mode.Fill ? fill : normal);
			base.Render();
		}
	}
}