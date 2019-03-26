using System;
using Lens.assets;
using Lens.game;
using Lens.graphics.gamerenderer;
using Lens.input;
using Lens.util;
using Lens.util.timer;
using Lens.util.tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Lens {
	public class Engine : Game {
		public static Version Version = new Version(0, 0, 0, 1, true, true);
		public static Engine Instance;
		public static GraphicsDeviceManager Graphics;
		public new static GraphicsDevice GraphicsDevice;
		public static Matrix ScreenMatrix;
		public static Matrix UiMatrix;
		public static float Time;
		public static float Delta;
		
		public FrameCounter Counter;
		public GameRenderer StateRenderer;
		public GameState State { get; private set; }
		public static Vector2 Viewport;
		public float Upscale;
		public float UiUpscale;

		private static Core.Core core;
		private const float FixedUpdateTime = 0.015f;
		private float time;
		public float Speed = 1;
		public float Flash;
		public float Freeze;
		public Color FlashColor = Color.White;
		public float Split;
		// Window.Title works only in Init, sadly
		private string tmpTitle;
		private GameState newState;
		
		public string Title {
			get => Window.Title;
			set => Window.Title = value;
		}

		public Engine(GameState state, string title, int width, int height, bool fullscreen) {
			Instance = this;
			tmpTitle = title;

			Graphics = new GraphicsDeviceManager(this);
			Graphics.GraphicsProfile = GraphicsProfile.HiDef;

			core = Core.Core.SelectCore(Window, Graphics);
			core.Init(width, height, fullscreen);

			Content.RootDirectory = "Content/";
			Assets.Content = Content;

			newState = state;
			
			Counter = new FrameCounter();
		}

		protected override void OnActivated(object sender, EventArgs args) {
			base.OnActivated(sender, args);
			State?.OnActivated();
		}

		protected override void OnDeactivated(object sender, EventArgs args) {
			base.OnDeactivated(sender, args);
			State?.OnDeactivated();
		}

		protected override void LoadContent() {
			GraphicsDevice = base.GraphicsDevice;
			GraphicsDevice.BlendState = BlendState.NonPremultiplied;
			Assets.Load();
		}
		
		protected override void UnloadContent() {
			State?.Destroy();
			StateRenderer?.Destroy();
			
			Assets.Destroy();
			Input.Destroy();
			Log.Close();

			Instance = null;
			Log.Info("Bye");
		}
		
		protected override void Initialize() {
			base.Initialize();
			Window.Title = tmpTitle;

			if (StateRenderer == null) {
				StateRenderer = new PixelPerfectGameRenderer();
			}
			
			UpdateView();

			Log.Open();
			Log.Info(tmpTitle);
			Log.Info(DateTime.Now.ToString("dd.MM.yyyy h:mm tt"));

			Input.Init();
		}

		protected override void Update(GameTime gameTime) {
			base.Update(gameTime);

			if (Input.Keyboard.WasPressed(Keys.F11)) {
				if (Graphics.IsFullScreen) {
					SetWindowed(Display.Width * 3, Display.Height * 3);
				} else {
					SetFullscreen();
				}
			}

			var dt = (float) gameTime.ElapsedGameTime.TotalSeconds * Speed;

			Delta = dt;
			Time += dt;

			if (Freeze < 0.01f) {
				time += (float) gameTime.ElapsedGameTime.TotalSeconds;
			}

			Split = Math.Max(0, Split - dt);
			Flash = Math.Max(0, Flash - dt * 20f);
			Freeze = Math.Max(0, Freeze - dt * 10f);

			while (time >= FixedUpdateTime) {
				time -= FixedUpdateTime;
	
				if (newState != null) {
					Log.Info("Setting state to " + newState.GetType().Name);

					State?.Destroy();
					State = newState;
					State?.Init();

					newState = null;
				}

				Assets.Update(FixedUpdateTime);
				Input.Update(FixedUpdateTime);
				Timer.Update(FixedUpdateTime);
				Tween.Update(FixedUpdateTime);

				State?.Update(FixedUpdateTime);
			}
			
			Counter.Update(dt);
		}

		public virtual void RenderUi() {
			
		}

		public void SetState(GameState state) {
			if (State == null) {
				State = state;
				State.Init();
			} else {
				newState = state;				
			}
		}

		public void Quit() {
			Exit();
		}
		
		protected override void Draw(GameTime gameTime) {
			StateRenderer.Render();
			base.Draw(gameTime);
		}

		public void SetWindowed(int width, int height) {
			core.SetWindowed(width, height);
			UpdateView();
		}

		public void SetFullscreen() {
			core.SetFullscreen();
			UpdateView();
		}
		
		public void UpdateView() {
			float screenWidth = Math.Max(Display.Width, base.GraphicsDevice.PresentationParameters.BackBufferWidth);
			float screenHeight = Math.Max(Display.Height, base.GraphicsDevice.PresentationParameters.BackBufferHeight);
			
			if (Graphics.IsFullScreen) {
				screenWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
				screenHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
			}

			Graphics.PreferredBackBufferWidth = (int) screenWidth;
			Graphics.PreferredBackBufferHeight = (int) screenHeight;
			Graphics.ApplyChanges();

			Upscale = Math.Min(screenWidth / Display.Width, screenHeight / Display.Height);
			UiUpscale = Math.Min(screenWidth / Display.UiWidth, screenHeight / Display.UiHeight);

			Viewport.X = (screenWidth - Upscale * Display.Width) / 2;
			Viewport.Y = (screenHeight - Upscale * Display.Height) / 2;

			float viewWidth;
			
			if (screenWidth / Display.Width > screenHeight / Display.Height) {
				viewWidth = screenHeight / Display.Height * Display.Width;
			} else {
				viewWidth = screenWidth;
			}
			
			ScreenMatrix = Matrix.CreateScale(Upscale) * Matrix.CreateTranslation(Viewport.X, Viewport.Y, 0);
			UiMatrix = Matrix.CreateScale(viewWidth / Display.UiWidth) * Matrix.CreateTranslation(Viewport.X, Viewport.Y, 0);
		}
	}
}