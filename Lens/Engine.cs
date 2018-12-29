using System;
using Lens.Asset;
using Lens.Graphics.GameRenderer;
using Lens.Inputs;
using Lens.Pattern.Observer;
using Lens.State;
using Lens.Util;
using Lens.Util.Camera;
using Lens.Util.Timer;
using Lens.Util.Tween;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Lens {
	public class Engine : Game {
		public static Version Version = new Version(0, 0, 0, 1, true);
		public static Engine Instance;
		public static GraphicsDeviceManager Graphics;
		public new static GraphicsDevice GraphicsDevice;
		public static float DeltaTime;
		public static GameTime GameTime;
		
		public GameRenderer StateRenderer { get; private set; }
		public GameState State { get; private set; }
		public static Vector2 Viewport;
		public float Upscale;

		private static Core.Core core;
		// Window.Title works only in Init, sadly
		private string tmpTitle;
		private GameState newState;
		
		public string Title {
			get { return Window.Title; }
			set { Window.Title = value; }
		}

		public Engine(GameState state, string title, int width, int height, bool fullscreen) {
			Instance = this;
			tmpTitle = $"Lens {Version}: {title}";

			Graphics = new GraphicsDeviceManager(this);
			core = Core.Core.SelectCore(Window, Graphics);
			core.Init(width, height, fullscreen);
			Assets.Content = Content;
			
			SetState(state);
		}

		protected override void LoadContent() {
			GraphicsDevice = base.GraphicsDevice;
			Assets.Load();
		}
		
		protected override void UnloadContent() {
			StateRenderer?.Destroy();
			
			Assets.Destroy();
			Input.Destroy();
			Log.Close();
			Subjects.Destroy();

			Instance = null;
		}
		
		protected override void Initialize() {
			base.Initialize();
			Window.Title = tmpTitle;

			if (StateRenderer == null) {
				StateRenderer = new PixelPerfectGameRenderer();
			}
			
			UpdateView();
			Subjects.Init();

			Log.Open();
			Log.Info(tmpTitle);
			Log.Info(DateTime.Now.ToString("MM/dd/yyyy h:mm tt"));

			Input.Init();
		}

		protected override void Update(GameTime gameTime) {
			base.Update(gameTime);

			float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;

			GameTime = gameTime;
			DeltaTime = dt;

			if (newState != null) {
				State?.Destroy();
				State = newState;
				State?.Init();
			
				newState = null;
			}

			Input.Update();
			Timer.Update(dt);
			Tween.Update(dt);
			
			State?.Update(dt);
			Subjects.Update();
		}

		public void SetState(GameState state) {
			newState = state;
		}

		public void Quit() {
			Exit();
		}
		
		protected override void Draw(GameTime gameTime) {
			StateRenderer.Render();
			base.Draw(gameTime);
		}

		public static void SetWindowed(int width, int height) {
			core.SetWindowed(width, height);
		}

		public static void SetFullscreen() {
			core.SetFullscreen();
		}
		
		public void UpdateView() {
			float screenWidth = Math.Max(Display.Width, base.GraphicsDevice.PresentationParameters.BackBufferWidth);
			float screenHeight = Math.Max(Display.Height, base.GraphicsDevice.PresentationParameters.BackBufferHeight);

			Graphics.PreferredBackBufferWidth = (int) screenWidth;
			Graphics.PreferredBackBufferHeight = (int) screenHeight;
			Graphics.ApplyChanges();

			Upscale = Math.Min(screenWidth / Display.Width, screenHeight / Display.Height);

			Viewport.X = (screenWidth - Upscale * Display.Width) / 2;
			Viewport.Y = (screenHeight - Upscale * Display.Height) / 2;
		}
	}
}