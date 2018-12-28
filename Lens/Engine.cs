using System;
using Lens.Asset;
using Lens.Graphics.GameRenderer;
using Lens.Inputs;
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
		public static GraphicsDevice GraphicsDev;
		public static float DeltaTime;
		public static GameTime GameTime;

		#region Game init
		
		public string Title {
			get { return Window.Title; }
			set { Window.Title = value; }
		}

		// Window.Title works only in Init, sadly
		private string tmpTitle;

		public GameState State { get; private set; }
		private GameState newState;

		public Engine(GameState state, string title, int width, int height, bool fullscreen) {
			Instance = this;
			tmpTitle = $"Lens {Version}: {title}";

			Graphics = new GraphicsDeviceManager(this);
			
#if PS4 || XBOXONE
			Graphics.PreferredBackBufferWidth = 1920;
			Graphics.PreferredBackBufferHeight = 1080;
#elif NSWITCH
			Graphics.PreferredBackBufferWidth = 1280;
			Graphics.PreferredBackBufferHeight = 720;
#else
			Window.AllowUserResizing = true;
			Window.ClientSizeChanged += OnClientSizeChanged;

			if (fullscreen) {
				Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
				Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
				Graphics.IsFullScreen = true;
			} else {
				Graphics.PreferredBackBufferWidth = width;
				Graphics.PreferredBackBufferHeight = height;
				Graphics.IsFullScreen = false;
			}
#endif

			Assets.Content = Content;
			setState(state);
		}
		
		protected override void Initialize() {
			base.Initialize();

			Window.Title = tmpTitle;
			GraphicsDev = GraphicsDevice;
			StateRenderer = new PixelPerfectGameRenderer();
			
			UpdateView();

			Log.Open();
			Log.Info(tmpTitle);
			Log.Info(DateTime.Now.ToString("MM/dd/yyyy h:mm tt"));

			Input.Init();
		}

		protected override void LoadContent() {
			GraphicsDev = GraphicsDevice;
			Assets.Load();
		}
		
		#endregion

		protected override void UnloadContent() {
			StateRenderer?.Destroy();
			Assets.Destroy();
			Input.Destroy();
			Log.Close();

			Instance = null;
		}

		protected override void Update(GameTime gameTime) {
			base.Update(gameTime);
			GameTime = gameTime;

			float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
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
		}

		public void setState(GameState state) {
			newState = state;
		}

		public void Quit() {
			Exit();
		}
		
		#region Rendering game

		public GameRenderer StateRenderer { get; private set; }

		protected override void Draw(GameTime gameTime) {
			StateRenderer?.Render();
			base.Draw(gameTime);
		}

		#endregion
		
		#region Screen management
		
		public static Viewport Viewport { get; private set; }
		public static Matrix ScreenMatrix  { get; private set; }
		private static bool resizing;
		private float width;
		private float height;

		public Matrix GetCombinedMatrix() {
			return Camera.Instance == null ? ScreenMatrix : Camera.Instance.Matrix * ScreenMatrix;
		}

#if !CONSOLE
		protected virtual void OnClientSizeChanged(object sender, EventArgs e) {
			if (Window.ClientBounds.Width > 0 && Window.ClientBounds.Height > 0 && !resizing) {
				resizing = true;

				Graphics.PreferredBackBufferWidth = Window.ClientBounds.Width;
				Graphics.PreferredBackBufferHeight = Window.ClientBounds.Height;
				UpdateView();

				resizing = false;
			}
		}
#endif
		
		public static void SetWindowed(int width, int height) {
#if !CONSOLE
			if (width > 0 && height > 0) {
				resizing = true;
				Graphics.PreferredBackBufferWidth = width;
				Graphics.PreferredBackBufferHeight = height;
				Graphics.IsFullScreen = false;
				Graphics.ApplyChanges();
				Log.Info("Going windowed " + width + ":" + height);
				resizing = false;
			}
#endif
		}

		public static void SetFullscreen() {
#if !CONSOLE
			resizing = true;
			Graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
			Graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
			Graphics.IsFullScreen = true;
			Graphics.ApplyChanges();
			Log.Info("Going to fullscreen");
			resizing = false;
#endif
		}

		private void UpdateView() {
			float screenWidth = Math.Max(Display.Width, GraphicsDevice.PresentationParameters.BackBufferWidth);
			float screenHeight = Math.Max(Display.Height, GraphicsDevice.PresentationParameters.BackBufferHeight);

			Graphics.PreferredBackBufferWidth = (int) screenWidth;
			Graphics.PreferredBackBufferHeight = (int) screenHeight;
			Graphics.ApplyChanges();

			if (screenWidth / Display.Width > screenHeight / Display.Height) {
				width = (int) (screenHeight / Display.Height * Display.Width);
				height = (int) screenHeight;
			}	else {
				width = (int) screenWidth;
				height = (int) (screenWidth / Display.Width * Display.Height);
			}

			ScreenMatrix = Matrix.CreateScale(width / Display.Width);

			Viewport = new Viewport {
				X = (int) (screenWidth / 2 - width / 2),
				Y = (int) (screenHeight / 2 - height / 2),
				Width = (int) width,
				Height = (int) height,
				MinDepth = 0,
				MaxDepth = 1
			};
		}

		#endregion
	}
}