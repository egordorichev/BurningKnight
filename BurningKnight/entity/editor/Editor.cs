using System;
using BurningKnight.debug;
using BurningKnight.entity.editor.command;
using BurningKnight.level;
using BurningKnight.level.biome;
using BurningKnight.level.tile;
using BurningKnight.save;
using BurningKnight.state;
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
		public Console Console;
		public EntitySelectWindow EntitySelectWindow;
		
		public bool ShowPanes = true;

		public bool UseDepth;
		public Vector2 CameraPosition;

		public override void Init() {
			base.Init();

			Console = new Console(Area);

			AlwaysVisible = true;
			AlwaysActive = true;

			Console.AddCommand(new NewCommand(this));
			Console.AddCommand(new LoadCommand(this));
			
			Commands = new CommandQueue {
				Editor = this
			};
			
			Engine.Instance.State.Ui.Add(Cursor = new EditorCursor {
				Editor = this
			});
			
			Engine.Instance.State.Ui.Add(Camera = new Camera(new CameraDriver()) {
				Position = CameraPosition
			});

			if (UseDepth) {
				SaveManager.Load(Area, SaveType.Level);
				Level = Run.Level;
			} else {
				Area.Add(Level = new RegularLevel(BiomeRegistry.Defined[Biome.Castle]) {
					Width = 32, Height = 32
				});

				Level.Setup();
				Level.Fill(Tile.FloorA);
				Level.TileUp();
			}

			Engine.Instance.State.Ui.Add(TileSelect = new TileSelect {
				Editor = this
			});

			Depth = Layers.InGameUi;
			EntitySelectWindow = new EntitySelectWindow(this);
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
			
			if (Engine.Version.Debug) {
				Tilesets.Update();
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

		public void RenderNative() {
			Console.Render();
			EntitySelectWindow.Render();
		}
	}
}