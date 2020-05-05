using System;
using BurningKnight.assets;
using BurningKnight.assets.lighting;
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
using Lens.util.timer;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;

namespace BurningKnight.state {
	public class CutsceneState : GameState {
		public Camera Camera;
		private float blackBarsSize = 50;
		private TextureRegion black;
		public Area TopUi;

		private string text;
		private float textW;
		private bool saying;
		private float a;

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

			Shaders.Ui.Parameters["black"].SetValue(0f);
			Shaders.Ui.Parameters["bx"].SetValue(0.333f);
			Shaders.Ui.Parameters["by"].SetValue(0.333f);

			Tween.To(1, 0, x => Shaders.Ui.Parameters["black"].SetValue(x), 0.7f, Ease.QuadIn);

			Area.Add(new GobboCutsceneController {
				State = this
			});
			
			Ui.Add(Camera = new Camera(new FollowingDriver()));
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
			if (saying) {
				return;
			}
			
			PrerenderShadows();
			base.Render();
		}

		public override void RenderUi() {
			base.RenderUi();
			
			if (blackBarsSize > 0.01f) {
				Graphics.Render(black, Vector2.Zero, 0, Vector2.Zero, new Vector2(Display.UiWidth + 1, blackBarsSize));
				Graphics.Render(black, new Vector2(0, Display.UiHeight + 1 - blackBarsSize), 0, Vector2.Zero, new Vector2(Display.UiWidth + 1, blackBarsSize + 1));
			}
			
			TopUi.Render();

			if (saying) {
				Graphics.Color.A = (byte) (a * 255f);
				Graphics.Print(text, Font.Medium, new Vector2((Display.UiWidth - textW) * 0.5f, Display.UiHeight * 0.5f - 4));
				Graphics.Color.A = 255;
			}
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
			Run.Update();
		}

		public void Say(string what, Action callback = null) {
			text = what;
			textW = Font.Medium.MeasureString(what).Width;
			
			Camera.Instance.Targets.Clear();
			Shaders.Ui.Parameters["bx"].SetValue(0.333f);
			Shaders.Ui.Parameters["by"].SetValue(0.333f);

			Tween.To(0, 1, x => Shaders.Ui.Parameters["black"].SetValue(x), 0.7f).OnEnd = () => {
				Shaders.Ui.Parameters["bx"].SetValue(0.333f);
				Shaders.Ui.Parameters["by"].SetValue(0.333f);

				a = 0;
				Tween.To(1, 0, x => a = x, 0.5f);

				saying = true;
				Shaders.Ui.Parameters["black"].SetValue(1f);

				Tween.To(0, 1, x => a = x, 0.5f).OnEnd = () => {
					Timer.Add(() => {
						saying = false;
						callback?.Invoke();
						Shaders.Ui.Parameters["black"].SetValue(0f);
						Tween.To(1, 0, x => Shaders.Ui.Parameters["black"].SetValue(x), 0.7f, Ease.QuadIn);
					}, 2f);
				};
			};
		}

		public void Transition(Action callback) {
			Camera.Instance.Targets.Clear();
			Shaders.Ui.Parameters["bx"].SetValue(0.333f);
			Shaders.Ui.Parameters["by"].SetValue(0.333f);

			Tween.To(0, 1, x => Shaders.Ui.Parameters["black"].SetValue(x), 0.7f).OnEnd = () => {
				Shaders.Ui.Parameters["bx"].SetValue(0.333f);
				Shaders.Ui.Parameters["by"].SetValue(0.333f);
				callback();
			};
		}
	}
}