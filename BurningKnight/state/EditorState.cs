using System;
using BurningKnight.assets;
using BurningKnight.assets.lighting;
using BurningKnight.entity.component;
using BurningKnight.level;
using BurningKnight.level.biome;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.ui.editor;
using BurningKnight.util;
using Lens;
using Lens.game;
using Lens.graphics;
using Lens.graphics.gamerenderer;
using Lens.input;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using VelcroPhysics.Utilities;
using Console = BurningKnight.debug.Console;

namespace BurningKnight.state {
	public class EditorState : GameState {
		public Level Level;
		public SettingsWindow Settings;
		public Camera Camera;
		public Console Console;
		
		public override void Init() {
			base.Init();

			Engine.EditingLevel = true;

			Physics.Init();
			Lights.Init();
			Tilesets.Load();

			Ui.Add(Camera = new Camera(new FollowingDriver()));
			
			Settings = new SettingsWindow(this);
			Console = new Console(Area);

			for (var i = 0; i < Level.Explored.Length; i++) {
				Level.Explored[i] = true;
			}
		}

		public override void Destroy() {
			base.Destroy();

			Physics.Destroy();
			Lights.Destroy();
			
			Engine.EditingLevel = false;
		}
		
		public override void Update(float dt) {
			base.Update(dt);
			Console.Update(dt);
			
			if (Input.Keyboard.IsDown(Keys.Space)) {
				Camera.Position -= Input.Mouse.PositionDelta;
				
				Camera.X = MathUtils.Clamp(Camera.X, -Display.Width / 2f, Level.Width * 16f - Display.Width / 2f);
				Camera.Y = MathUtils.Clamp(Camera.Y, -Display.Height / 2f, Level.Height * 16f - Display.Height / 2f);
			}
		}

		private void PrerenderShadows() {
			var renderer = (PixelPerfectGameRenderer) Engine.Instance.StateRenderer;
			
			renderer.End();
			renderer.BeginShadows();

			foreach (var e in Area.Tags[Tags.HasShadow]) {
				if (e.AlwaysVisible || e.OnScreen) {
					e.GetComponent<ShadowComponent>().Callback();
				}
			}
			
			renderer.EndShadows();
			renderer.Begin();
		}

		public override void Render() {
			PrerenderShadows();
			base.Render();

			if (Settings.Grid) {
				var gridSize = 16;
				var off = (Camera.Instance.TopLeft - new Vector2(0, 8));
				var color = new Color(1f, 1f, 1f, 0.5f);

				for (float x = Math.Max(0, off.X - off.X % gridSize); x <= off.X + Display.Width && x <= Level.Width * 16; x += gridSize) {
					Graphics.Batch.DrawLine(x, off.Y, x, off.Y + Display.Height + gridSize, color);
				}

				for (float y = Math.Max(0, off.Y - off.Y % gridSize); y <= off.Y + Display.Height && y <= Level.Height * 16; y += gridSize) {
					Graphics.Batch.DrawLine(off.X, y, off.X + Display.Width + gridSize, y, color);
				}
			}

			if (Settings.EditingTiles) {
				var mouse = Input.Mouse.GamePosition;
				var color = new Color(1f, 0.5f, 0.5f, 1f);
				var fill = new Color(1f, 0.5f, 0.5f, 0.5f);

				mouse.X = (float) (Math.Floor(mouse.X / 16) * 16);
				mouse.Y = (float) (Math.Floor(mouse.Y / 16) * 16);

				if (Settings.CurrentInfo.Tile.Matches(TileFlags.WallLayer)) {
					mouse.Y -= 8;
					Graphics.Batch.FillRectangle(mouse, new Vector2(16, 24), fill);
					mouse.Y += 16;
					Graphics.Batch.DrawRectangle(mouse, new Vector2(16, 8), new Color(1f, 0.7f, 0.7f, 1f));
					mouse.Y -= 16;
					Graphics.Batch.DrawRectangle(mouse, new Vector2(16), color);
				} else {
					Graphics.Batch.FillRectangle(mouse, new Vector2(16, 16), fill);
					Graphics.Batch.DrawRectangle(mouse, new Vector2(16), color);
				}
			} else {
				if (Settings.HoveredEntity != null) {
					Graphics.Batch.DrawRectangle(Settings.HoveredEntity.Position - new Vector2(1), new Vector2(Settings.HoveredEntity.Width + 2, Settings.HoveredEntity.Height + 2), new Color(0.7f, 0.7f, 1f, 1f));
				}
				
				if (Settings.CurrentEntity != null) {
					var e = Settings.CurrentEntity;
					Graphics.Batch.DrawRectangle(e.Position - new Vector2(1), new Vector2(e.Width + 2, e.Height + 2), new Color(0.7f, 1f, 0.7f, 1f));
				}
			}
		}

		public override void RenderNative() {
			ImGuiHelper.Begin();
			Settings.Render();
			Console.Render();
			AreaDebug.Render(Area);
			ImGuiHelper.End();
			
			Graphics.Batch.Begin();
			Graphics.Batch.DrawCircle(new CircleF(Mouse.GetState().Position, 3f), 8, Color.White);
			Graphics.Batch.End();
		}
	}
}