using BurningKnight.assets;
using BurningKnight.assets.lighting;
using BurningKnight.assets.prefabs;
using BurningKnight.entity.component;
using BurningKnight.level;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.ui.editor;
using BurningKnight.ui.imgui;
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
		public EditorWindow Editor;
		public Camera Camera;
		public Console Console;
		
		public override void Init() {
			base.Init();

			Engine.Instance.StateRenderer.GameEffect = null;
			Engine.Instance.StateRenderer.UiEffect = null;
			
			WindowManager.LevelEditor = true;

			Engine.EditingLevel = true;

			Physics.Init();
			Lights.Init();
			Tilesets.Load();

			Ui.Add(Camera = new Camera(new FollowingDriver()));
			Console = new Console(Area);

			Editor = new EditorWindow(new Editor {
				Area = Area,
				Level = Level,
				Camera = Camera
			});
			

			Level = Editor.Editor.Level;
			for (var i = 0; i < Level.Explored.Length; i++) {
				Level.Explored[i] = true;
			}
		}

		public override void Destroy() {
			base.Destroy();

			Physics.Destroy();
			Lights.Destroy();
			
			Engine.EditingLevel = false;
			Prefabs.Reload();
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

			renderer.EnableClip = false;
			renderer.End();
			renderer.BeginShadows();

			foreach (var e in Area.Tagged[Tags.HasShadow]) {
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
			Physics.Render();
			Editor.RenderInGame();
		}


		public override void RenderNative() {
			ImGuiHelper.Begin();
			Editor.Render();
			Console.Render();
			WindowManager.Render(Area);
			ImGuiHelper.End();			
			
			Graphics.Batch.Begin();
			Graphics.Batch.DrawCircle(new CircleF(Mouse.GetState().Position, 3f), 8, Color.White);
			Graphics.Batch.End();
		}
	}
}