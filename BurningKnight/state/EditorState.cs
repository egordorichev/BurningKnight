using BurningKnight.assets;
using BurningKnight.assets.lighting;
using BurningKnight.debug;
using BurningKnight.level;
using BurningKnight.level.biome;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.ui.editor;
using BurningKnight.util;
using Lens;
using Lens.game;
using Lens.graphics;
using Lens.input;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

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

			Area.Add(Level = new RegularLevel {
				Width = 32, Height = 32,
				NoLightNoRender = false,
				DrawLight = false
			});
			
			Ui.Add(Camera = new Camera(new FollowingDriver()));
			
			Level.SetBiome(BiomeRegistry.Get(Biome.Castle));
			Level.Setup();
			Level.Fill(Tile.FloorA);
			Level.TileUp();
			
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
			}
		}

		public override void Render() {
			base.Render();

			if (Settings.Grid) {
				var gridSize = 16;
				var off = (Camera.Instance.TopLeft - new Vector2(0, 8));
				var color = new Color(1f, 1f, 1f, 0.5f);
				
				for (float x = off.X - off.X % gridSize; x <= off.X + Display.Width; x += gridSize) {
					Graphics.Batch.DrawLine(x, 0, x, Display.Height, color);
				}

				for (float y = off.Y - off.Y % gridSize; y <= off.Y + Display.Height; y += gridSize) {
					Graphics.Batch.DrawLine(0, y, Display.Width, y, color);
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