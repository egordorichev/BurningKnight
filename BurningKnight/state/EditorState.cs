using System;
using BurningKnight.entity.editor;
using BurningKnight.entity.level;
using BurningKnight.entity.level.biome;
using Lens;
using Lens.game;
using Lens.graphics;
using Lens.input;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using MonoGame.Extended;

namespace BurningKnight.state {
	public class EditorState : GameState {
		private EditorCursor cursor;
		
		public override void Init() {
			base.Init();
			Tilesets.Load();

			Ui.Add(cursor = new EditorCursor());
			Ui.Add(new Camera(new CameraDriver()));

			var level = new RegularLevel(BiomeRegistry.Defined[Biome.Castle]);

			level.Width = 32;
			level.Height = 32;
			
			Area.Add(level);
			
			level.Setup();
		}

		public override void Render() {
			base.Render();

			var camera = Camera.Instance;
			
			// Cache the condition
			var toX = Run.Level.GetRenderRight(camera);
			var toY = Run.Level.GetRenderTop(camera);

			var color = new Color(1f, 1f, 1f, 0.5f);
			
			for (int y = Run.Level.GetRenderBottom(camera) - 1; y >= toY; y--) {
				Graphics.Batch.DrawLine(0, y * 16, Display.Width, y * 16, color);
			}
			
			for (int x = Run.Level.GetRenderLeft(camera); x < toX; x++) {
				Graphics.Batch.DrawLine(x * 16, 0, x * 16, Display.Height, color);
			}

			if (cursor.CurrentMode != EditorCursor.Mode.Drag) {
				Graphics.Batch.DrawRectangle(new RectangleF(
					(float) (Math.Floor(Input.Mouse.GamePosition.X / 16) * 16), 
					(float) (Math.Floor(Input.Mouse.GamePosition.Y / 16) * 16), 16, 16
				), ColorUtils.WhiteColor);
			}
		}
	}
}