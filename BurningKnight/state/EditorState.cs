using BurningKnight.assets;
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

			Physics.Init();
			Tilesets.Load();

			Area.Add(Level = new RegularLevel {
				Width = 32, Height = 32,
				NoLightNoRender = false
			});

			Ui.Add(Camera = new Camera(new FollowingDriver()));
			
			Level.SetBiome(BiomeRegistry.Get(Biome.Castle));
			Level.Setup();
			Level.Fill(Tile.FloorA);
			Level.TileUp();
			
			Settings = new SettingsWindow(this);
			Console = new Console(Area);
		}

		public override void Destroy() {
			base.Destroy();

			Physics.Destroy();
		}
		
		public override void Update(float dt) {
			base.Update(dt);
			
			Console.Update(dt);
			
			Camera.Width = Engine.Instance.GetScreenWidth();
			Camera.Height = Engine.Instance.GetScreenHeight();
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

		public override bool NativeRender() {
			return true;
		}
	}
}