using BurningKnight.assets;
using BurningKnight.assets.lighting;
using BurningKnight.entity;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.physics;
using BurningKnight.ui;
using BurningKnight.util;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.game;
using Lens.graphics;
using Lens.input;
using Lens.util.camera;
using Lens.util.tween;
using Console = BurningKnight.debug.Console;

namespace BurningKnight.state {
	public class InGameState : GameState, Subscriber {
		private bool pausedByMouseOut;
		private bool pausedByLostFocus;
		private float blur;
		private TextureRegion fog;
		private float time;
		private UiPane pauseMenu;
		private UiPane gameOverMenu;
		private bool died;
		
		public InGameState(Area area) {
			Area = area;
			Area.EventListener.Subscribe<DiedEvent>(this);
		}
		
		public override void Init() {
			base.Init();
			SetupUi();

			for (int i = 0; i < 30; i++) {
				Area.Add(new WindFx());
			}

			fog = Textures.Get("noise");
		}

		public override void Destroy() {
			Physics.Destroy();
			Lights.Destroy();
			// Fixme: enable
			// SaveManager.SaveAll(Area);
			Area = null;
			
			Shaders.Screen.Parameters["split"].SetValue(0f);
			Shaders.Screen.Parameters["blur"].SetValue(0f);

			// Clears the area, but we don't want that, cause we are still saving
			base.Destroy();
		}

		protected override void OnPause() {
			base.OnPause();

			if (died) {
				return;
			}
			
			// fixme: quadOut doesnt feel smooth as tween for the pauseMenu.Y
			// it seems like its broken
			Tween.To(this, new {blur = 1}, 0.25f);
			Tween.To(0, pauseMenu.Y, x => pauseMenu.Y = x, 0.25f);
		}

		protected override void OnResume() {
			base.OnResume();
			
			if (died) {
				return;
			}
			
			Tween.To(this, new {blur = 0}, 0.25f);
			Tween.To(-Display.UiHeight, pauseMenu.Y, x => pauseMenu.Y = x, 0.25f);

			pausedByMouseOut = false;
		}

		public override void OnActivated() {
			base.OnActivated();

			if (Paused && pausedByLostFocus) {
				Paused = false;
			}
		}

		public override void OnDeactivated() {
			base.OnDeactivated();

			Paused = true;
			pausedByLostFocus = true;
			pausedByMouseOut = false;
		}

		public override void Update(float dt) {
			var inside = Engine.GraphicsDevice.Viewport.Bounds.Contains(Input.Mouse.CurrentState.Position);
			
			Shaders.Screen.Parameters["split"].SetValue(Engine.Instance.Split);
			Shaders.Screen.Parameters["blur"].SetValue(blur);
			
			if (!Paused && !inside && !Engine.Version.Debug) {
				Paused = true;
				pausedByMouseOut = true;
			} else if (Paused && pausedByMouseOut && inside) {
				Paused = false;
			}
			
			if (!Paused) {
				time += dt;
				Physics.Update(dt);
				base.Update(dt);
			} else {
				Ui.Update(dt);
			}

			if (Input.WasPressed(Controls.Pause)) {
				Paused = !Paused;
			}
		}

		public override void Render() {
			base.Render();
			Physics.Render();

			var shader = Shaders.Fog;
			Shaders.Begin(shader);

			var wind = WindFx.CalculateWind();
			
			shader.Parameters["time"].SetValue(time * 0.01f);
			shader.Parameters["tx"].SetValue(wind.X * -0.1f);
			shader.Parameters["ty"].SetValue(wind.Y * -0.1f);
			shader.Parameters["cx"].SetValue(Camera.Instance.Position.X / 512f);
			shader.Parameters["cy"].SetValue(Camera.Instance.Position.Y / 512f);
		
			Graphics.Render(fog, Camera.Instance.TopLeft);
			
			Shaders.End();
		}

		public override void RenderUi() {
			base.RenderUi();
			Graphics.Print($"{Engine.Instance.Counter.AverageFramesPerSecond}", Font.Small, 1, 1);
		}

		private void SetupUi() {
			Ui.Add(new Camera(new FollowingDriver()));
			
			var cursor = new Cursor();
			Ui.Add(cursor);

			var player = LocalPlayer.Locate(Area);
			
			Camera.Instance.Follow(player, 1f);
			Camera.Instance.Follow(cursor, 1f);
			Camera.Instance.Jump();

			Ui.Add(new Console(Area));
			Ui.Add(new UiInventory(player));
			
			Ui.Add(pauseMenu = new UiPane {
				Y = -Display.UiHeight				
			});

			var space = 32f;
			var start = (Display.UiHeight - space * 2 - 14 * 3) / 2f;

			pauseMenu.Add(new UiButton {
				LocaleLabel = "resume",
				CenterX = Display.UiWidth / 2f,
				Y = start,
				Click = () => Paused = false
			});
			
			pauseMenu.Add(new UiButton {
				LocaleLabel = "settings",
				CenterX = Display.UiWidth / 2f,
				Y = start + space
			});
			
			pauseMenu.Add(new UiButton {
				LocaleLabel = "back_to_menu",
				CenterX = Display.UiWidth / 2f,
				Y = start + space * 2,
				Click = () => Engine.Instance.SetState(new MenuState())
			});
			
			
			Ui.Add(gameOverMenu = new UiPane {
				Y = -Display.UiHeight				
			});
			
			space = 32f;
			start = (Display.UiHeight - space * 2 - 14 * 4) / 2f;
			
			gameOverMenu.Add(new UiLabel {
				LocaleLabel = "death_message",
				CenterX = Display.UiWidth / 2f,
				Y = start
			});
			
			gameOverMenu.Add(new UiButton {
				LocaleLabel = "restart",
				CenterX = Display.UiWidth / 2f,
				Y = start + space * 2,
				Click = () => Engine.Instance.SetState(new LoadState())
			});
			
			gameOverMenu.Add(new UiButton {
				LocaleLabel = "back_to_menu",
				CenterX = Display.UiWidth / 2f,
				Y = start + space * 3,
				Click = () => Engine.Instance.SetState(new MenuState())
			});
		}

		public bool HandleEvent(Event e) {
			if (died) {
				return false;
			}
			
			if (e is DiedEvent ded && ded.Who is LocalPlayer) {
				died = true;
				
				Tween.To(this, new {blur = 1}, 0.5f);
				Tween.To(0, gameOverMenu.Y, x => gameOverMenu.Y = x, 0.5f);
			}

			return false;
		}
	}
}