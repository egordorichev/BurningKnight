using System;
using BurningKnight.entity.editor.command;
using BurningKnight.entity.level;
using BurningKnight.entity.level.biome;
using BurningKnight.state;
using Lens;
using Lens.entity;
using Lens.graphics;
using Lens.input;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace BurningKnight.entity.editor {
	public class Editor : Entity {
		public EditorCursor Cursor;
		public CommandQueue Commands;
		public Camera Camera;
		public Level Level;
		public TileSelect TileSelect;

		public override void Init() {
			base.Init();

			AlwaysVisible = true;
			AlwaysActive = true;
			
			Commands = new CommandQueue {
				Editor = this
			};
			
			TileSelect = new TileSelect();
			
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
		}
		
		public override void Render() {
			base.Render();

			// Cache the condition
			var toX = Level.GetRenderRight(Camera);
			var toY = Level.GetRenderTop(Camera);

			var color = new Color(1f, 1f, 1f, 0.5f);
			
			for (int y = Run.Level.GetRenderBottom(Camera) - 1; y >= toY; y--) {
				Graphics.Batch.DrawLine(0, y * 16, Display.Width, y * 16, color);
			}
			
			for (int x = Run.Level.GetRenderLeft(Camera); x < toX; x++) {
				Graphics.Batch.DrawLine(x * 16, 0, x * 16, Display.Height, color);
			}

			if (Cursor.CurrentMode != EditorCursor.Mode.Drag) {
				var x = (int) Math.Floor(Input.Mouse.GamePosition.X / 16); 
				var y = (int) Math.Floor(Input.Mouse.GamePosition.Y / 16);

				if (Level.IsInside(x, y)) {
					Graphics.Batch.DrawRectangle(new RectangleF(x * 16, y * 16, 16, 16), ColorUtils.WhiteColor);
				}
			}
		}
	}
}