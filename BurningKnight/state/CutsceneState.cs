using BurningKnight.assets;
using BurningKnight.assets.lighting;
using BurningKnight.debug;
using BurningKnight.entity.component;
using BurningKnight.entity.cutscene.controller;
using BurningKnight.physics;
using BurningKnight.ui.imgui;
using BurningKnight.util;
using Lens;
using Lens.entity;
using Lens.game;
using Lens.graphics;
using Lens.graphics.gamerenderer;
using Lens.input;
using Lens.util.camera;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BurningKnight.state {
	public class CutsceneState : GameState {
		public Console Console;
		public Camera Camera;
		private float blackBarsSize = 50;
		private TextureRegion black;
		public Area TopUi;

		public CutsceneState(Area area) {
			Area = area;
		}

		public override void Destroy() {
			Lights.Destroy();
			Area.Destroy();
			Area = null;

			Physics.Destroy();
			base.Destroy();
			TopUi.Destroy();
		}

		public override void Init() {
			base.Init();
			TopUi = new Area();

			Shaders.Ui.Parameters["black"].SetValue(1f);

			Area.Add(new GobboCutsceneController());
			Ui.Add(Camera = new Camera(new FollowingDriver()));
			Console = new Console(Area);
			
			Camera.Position = new Vector2(Display.Width / 2f, Display.Height / 2f) + new Vector2(32);
			
			black = CommonAse.Ui.GetSlice("black");
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
		}

		public override void RenderNative() {
			ImGuiHelper.Begin();
			Console.Render();
			WindowManager.Render(Area);
			ImGuiHelper.End();			
			
			Graphics.Batch.Begin();
			Graphics.Batch.DrawCircle(new CircleF(Mouse.GetState().Position, 3f), 8, Color.White);
			Graphics.Batch.End();
		}

		public override void RenderUi() {
			base.RenderUi();
			
			if (blackBarsSize > 0.01f) {
				Graphics.Render(black, Vector2.Zero, 0, Vector2.Zero, new Vector2(Display.UiWidth + 1, blackBarsSize));
				Graphics.Render(black, new Vector2(0, Display.UiHeight + 1 - blackBarsSize), 0, Vector2.Zero, new Vector2(Display.UiWidth + 1, blackBarsSize + 1));
			}
			
			TopUi.Render();
		}

		public override void Update(float dt) {
			base.Update(dt);
			float speed = dt * 120f;
				
			if (Input.Keyboard.IsDown(Keys.NumPad4)) {
				Camera.Instance.PositionX -= speed;
			}
				
			if (Input.Keyboard.IsDown(Keys.NumPad6)) {
				Camera.Instance.PositionX += speed;
			}
				
			if (Input.Keyboard.IsDown(Keys.NumPad8)) {
				Camera.Instance.PositionY -= speed;
			}
				
			if (Input.Keyboard.IsDown(Keys.NumPad2)) {
				Camera.Instance.PositionY += speed;
			}
			
			Physics.Update(dt);
			TopUi.Update(dt);
		}
	}
}