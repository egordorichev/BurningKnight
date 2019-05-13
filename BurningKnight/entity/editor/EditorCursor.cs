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
		public const int Normal = 0;
		public const int Fill = 1;
		public const int Drag = 2;
		public const int Entity = 3;
		
		private TextureRegion hand;
		private TextureRegion normal;
		private TextureRegion fill;
		private Entity entity;
		private string entityType;

		public int CurrentMode;
		public bool Draggging;
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

			Draggging = Input.Keyboard.IsDown(Keys.Space);

			if ((Draggging || CurrentMode == Drag) && Input.Mouse.CheckLeftButton && Input.Mouse.WasMoved) {
				Camera.Instance.Position -= Input.Mouse.RawPositionDelta;
			}

			if (Input.Keyboard.WasPressed(Keys.F) || Input.Keyboard.WasPressed(Keys.G)) {
				CurrentMode = Fill;
			}

			if (Input.Keyboard.WasPressed(Keys.N) || Input.Keyboard.WasPressed(Keys.B)) {
				CurrentMode = Normal;
			}

			if (Input.Keyboard.WasPressed(Keys.E)) {
				CurrentMode = Entity;
			}

			if (CurrentMode == Entity) {
				var mouse = Input.Mouse.GamePosition;

				if (Editor.SettingsWindow.SnapToGrid) {
					mouse.X = (float) (Math.Floor(mouse.X / 16) * 16);
					mouse.Y = (float) (Math.Floor(mouse.Y / 16) * 16);
				}
				
				if (Editor.SettingsWindow.Center) {
					entity.Center = mouse;
				} else {
					entity.Position = mouse;
				}
			}
		}

		public void OnClick(Vector2 pos) {
			if (Draggging || CurrentMode == Drag) {
				return;
			}

			var tile = Editor.SettingsWindow.CurrentTile;
			var x = (int) Math.Floor(pos.X / 16);
			var y = (int) Math.Floor(pos.Y / 16);

			if (!Editor.Level.IsInside(x, y)) {
				return;
			}
			
			if (CurrentMode == Entity) {
				entity = (Entity) Activator.CreateInstance(Type.GetType(entityType, true, false));
				return;
			}
			
			if (CurrentMode == Normal) {
				if (Editor.Level.Get(x, y, tile.Matches(TileFlags.LiquidLayer)) != tile) {
					Editor.Commands.Do(new SetCommand {
						X = x,
						Y = y,
						Tile = tile
					});
				}
			} else if (CurrentMode == Fill) {
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
			if (CurrentMode == Entity) {
				return;
			}
			
			Region = CurrentMode == Drag || Draggging ? hand : (CurrentMode == Fill ? fill : normal);
			base.Render();
		}
	}
}