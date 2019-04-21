using System.Threading;
using BurningKnight.assets;
using BurningKnight.assets.lighting;
using BurningKnight.entity;
using BurningKnight.entity.component;
using BurningKnight.entity.creature.player;
using BurningKnight.entity.events;
using BurningKnight.entity.fx;
using BurningKnight.level.paintings;
using BurningKnight.level.tile;
using BurningKnight.physics;
using BurningKnight.save;
using BurningKnight.ui;
using BurningKnight.util;
using Lens;
using Lens.assets;
using Lens.entity;
using Lens.game;
using Lens.graphics;
using Lens.graphics.gamerenderer;
using Lens.input;
using Lens.util.camera;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using MonoGame.Extended;
using Console = BurningKnight.debug.Console;

namespace BurningKnight.state {
	public class InGameState : GameState, Subscriber {
		private const float AutoSaveInterval = 60f;
		
		private bool pausedByMouseOut;
		private bool pausedByLostFocus;
		private float blur;
		private TextureRegion fog;
		private float time;
		private UiPane pauseMenu;
		private UiPane gameOverMenu;
		private bool died;
		private Cursor cursor;
		private float saveTimer;
		private SaveIndicator indicator;
		private bool saving;

		private Painting painting;

		public Painting CurrentPainting {
			set {
				painting = value;
				Paused = painting != null;
			}

			get => painting;
		}
		
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
			Lights.Destroy();

			if (died) {
				SaveManager.Save(Area, SaveType.Global, true);
			} else {
				SaveManager.Save(Area, SaveType.Global, true);
				SaveManager.Save(Area, SaveType.Game, true);
				SaveManager.Save(Area, SaveType.Level, true);
				SaveManager.Save(Area, SaveType.Player, true);
			}
			
			Area.Destroy();
			Area = null;
			Physics.Destroy();
			
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

			if (painting == null) {
				Tween.To(0, pauseMenu.Y, x => pauseMenu.Y = x, 0.25f);
			}
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

			if (painting != null) {
				if (Input.WasPressed(Controls.Pause) || Input.WasPressed(Controls.Interact) || Input.WasPressed(Controls.Use)) {
					painting.Remove();
				}
			} else {
				if (Input.WasPressed(Controls.Pause)) {
					Paused = !Paused;
				}
			}
			
			if (Engine.Version.Debug) {
				UpdateDebug(dt);
				Tilesets.Update();
			}

			Run.Update();

			if (Settings.Autosave && !saving) {
				saveTimer += dt;

				if (saveTimer >= AutoSaveInterval) {
					saveTimer = 0;
					saving = true;

					var thread = new Thread(() => {
						indicator.HandleEvent(new SaveStartedEvent());
													
						SaveManager.Save(Area, SaveType.Game, false, null, false, false);
						SaveManager.Save(Area, SaveType.Level, false, null, false, false);
						SaveManager.Save(Area, SaveType.Player, false, null, false, false);
						
						indicator.HandleEvent(new SaveEndedEvent());
						saving = false;
					});

					thread.Priority = ThreadPriority.Lowest;
					thread.Start();
				}
			}
		}

		private void UpdateDebug(float dt) {
			if (Input.Keyboard.WasPressed(Keys.NumPad7)) {
				Engine.Instance.SetState(new EditorState {
					Depth = Run.Depth,
					UseDepth = true,
					CameraPosition = Camera.Instance.Position
				});

				return;
			}

			if (Input.Keyboard.WasPressed(Keys.NumPad9)) {
				SaveManager.Delete(SaveType.Game, SaveType.Level, SaveType.Player);
				Engine.Instance.SetState(new LoadState());
				died = true;

				return;
			}

			if (Input.Keyboard.WasPressed(Keys.F2)) {
				Settings.ShowFps = !Settings.ShowFps;
			}
			
			if (Input.Keyboard.WasPressed(Keys.F3)) {
				Settings.HideUi = !Settings.HideUi;
			}

			if (Input.Keyboard.WasPressed(Keys.F4)) {
				Settings.HideCursor = !Settings.HideCursor;
			}
			
			if (Input.Keyboard.WasPressed(Keys.NumPad3)) {
				var level = Run.Level;

				for (var i = 0; i < level.Explored.Length; i++) {
					level.Explored[i] = true;
				}
			}

			if (Input.Keyboard.WasPressed(Keys.NumPad0)) {
				Camera.Instance.Detached = !Camera.Instance.Detached;
			}

			if (Camera.Instance.Detached) {
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
			}

			float s = dt * 10f;

			if (Input.Keyboard.WasPressed(Keys.I)) {
				Camera.Instance.TextureZoom -= s;
			}
			
			if (Input.Keyboard.WasPressed(Keys.O)) {
				Camera.Instance.TextureZoom = 1;
			}
			
			if (Input.Keyboard.WasPressed(Keys.P)) {
				Camera.Instance.TextureZoom += s;
			}
			
			if (Input.Keyboard.WasPressed(Keys.J)) {
				Camera.Instance.Zoom -= s;
			}
			
			if (Input.Keyboard.WasPressed(Keys.K)) {
				Camera.Instance.Zoom = 1;
			}
			
			if (Input.Keyboard.WasPressed(Keys.L)) {
				Camera.Instance.Zoom += s;
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
		
		private void RenderFog() {
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
		
		public override void Render() {
			PrerenderShadows();
			base.Render();
			Physics.Render();
			RenderFog();
		}

		public override void RenderUi() {
			if (painting != null) {
				painting.RenderUi();
			}
			
			if (Settings.HideUi) {
				cursor.Render();
				return;
			}
			
			base.RenderUi();

			if (Settings.ShowFps) {
				var c = Engine.Instance.Counter.AverageFramesPerSecond;
				Color color;

				if (c >= 59) {
					color = new Color(0f, 1f, 0f, 1f);
				} else if (c >= 49) {
					color = new Color(1f, 1f, 0f, 1f);
				} else {
					color = new Color(1f, 0f, 0f, 1f);
				}
				
				Graphics.Color = color;
				Graphics.Print($"{c}", Font.Small, 1, 1);
				Graphics.Color = ColorUtils.WhiteColor;
			}
		}

		private void SetupUi() {
			Ui.Add(new Camera(new FollowingDriver()));
			
			cursor = new Cursor();
			Ui.Add(cursor);
			
			Ui.Add(indicator = new SaveIndicator());

			var player = LocalPlayer.Locate(Area);
			
			Camera.Instance.Follow(player, 1f, true);
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
				RelativeCenterX = Display.UiWidth / 2f,
				RelativeY = start,
				Click = () => Paused = false
			});
			
			pauseMenu.Add(new UiButton {
				LocaleLabel = "settings",
				RelativeCenterX = Display.UiWidth / 2f,
				RelativeY = start + space
			});
			
			pauseMenu.Add(new UiButton {
				LocaleLabel = "back_to_hub",
				RelativeCenterX = Display.UiWidth / 2f,
				RelativeY = start + space * 2,
				Click = () => Run.Depth = -1
			});
			
			pauseMenu.Setup();
			
			Ui.Add(gameOverMenu = new UiPane {
				Y = -Display.UiHeight
			});
			
			space = 32f;
			start = (Display.UiHeight - space * 2 - 14 * 4) / 2f;
			
			gameOverMenu.Add(new UiLabel {
				LocaleLabel = "death_message",
				RelativeCenterX = Display.UiWidth / 2f,
				RelativeY = start
			});
			
			gameOverMenu.Add(new UiButton {
				LocaleLabel = "restart",
				RelativeCenterX = Display.UiWidth / 2f,
				RelativeY = start + space * 2,
				Click = () => Run.Depth = 0
			});
			
			gameOverMenu.Add(new UiButton {
				LocaleLabel = "back_to_hub",
				RelativeCenterX = Display.UiWidth / 2f,
				RelativeY = start + space * 3,
				Click = () => Run.Depth = -1
			});

			gameOverMenu.Setup();
		}

		public bool HandleEvent(Event e) {
			if (died) {
				return false;
			}
			
			if (e is DiedEvent ded && ded.Who is LocalPlayer) {
				died = true;
				
				Tween.To(this, new {blur = 1}, 0.5f);
				Tween.To(0, gameOverMenu.Y, x => gameOverMenu.Y = x, 0.5f);

				new Thread(() => {
					SaveManager.Delete(SaveType.Player, SaveType.Level, SaveType.Game);
				}).Start();
			}

			return false;
		}
	}
}