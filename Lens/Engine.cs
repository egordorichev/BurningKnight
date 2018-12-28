using System;
using Lens.Asset;
using Lens.Inputs;
using Lens.State;
using Lens.Util;
using Lens.Util.Timer;
using Lens.Util.Tween;
using Microsoft.Xna.Framework;

namespace Lens {
	public class Engine : Game {
		public static Version Version = new Version(0, 0, 0, 1, true);
		public static Engine Instance;
		public static GraphicsDeviceManager Graphics;
		public static float DeltaTime;

		public string Title {
			get { return Window.Title; }
			set { Window.Title = value; }
		}

		// Window.Title works only in Init, sadly
		private string tmpTitle;

		private GameState state;
		private GameState newState;
		
		public Engine(GameState state, string title, int width, int height, bool fullscreen) {
			Instance = this;
			tmpTitle = $"Lens {Version}: {title}";

			Graphics = new GraphicsDeviceManager(this) {
				PreferredBackBufferWidth = width,
				PreferredBackBufferHeight = height,
				IsFullScreen = fullscreen
			};

			Window.AllowUserResizing = true;
			Assets.Content = Content;
			
			setState(state);
		}

		protected override void Initialize() {
			base.Initialize();

			Window.Title = tmpTitle;
			
			Log.Open();
			Log.Info(tmpTitle);
			Log.Info(DateTime.Now.ToString("MM/dd/yyyy h:mm tt"));
			
			Input.Init();
		}

		protected override void LoadContent() {
			Assets.Load();
		}

		protected override void UnloadContent() {
			Assets.Destroy();
			Input.Destroy();
			Log.Close();
		}

		protected override void Update(GameTime gameTime) {
			base.Update(gameTime);
			
			float dt = (float) gameTime.ElapsedGameTime.TotalSeconds;
			DeltaTime = dt;

			if (newState != null) {
				state?.Destroy();
				state = newState;
				state?.Init();
				newState = null;
			}

			Input.Update();
			Timer.Update(dt);
			Tween.Update(dt);
			state?.Update(dt);
		}

		protected override void Draw(GameTime gameTime) {
			state?.Render();
			base.Draw(gameTime);
		}

		public void setState(GameState state) {
			newState = state;
		}

		public void Quit() {
			Exit();
		}
	}
}