using Microsoft.Xna.Framework;

namespace Lens {
	public class Engine : Game {
		public static Version Version = new Version(0, 0, 0, 1, true);
		public static Engine Instance;
		public static GraphicsDeviceManager Graphics;

		public string Title {
			get { return Window.Title; }
			set { Window.Title = value; }
		}
		

		public Engine(string title, int width, int height, bool fullscreen) {
			Instance = this;
			
			Window.Title = title;

			Graphics = new GraphicsDeviceManager(this) {
				PreferredBackBufferWidth = width,
				PreferredBackBufferHeight = height,
				IsFullScreen = fullscreen
			};
		}

		protected override void Initialize() {
			base.Initialize();
		}

		protected override void LoadContent() {
			
		}

		protected override void UnloadContent() {
		}

		protected override void Update(GameTime gameTime) {
			base.Update(gameTime);
		}

		protected override void Draw(GameTime gameTime) {
			GraphicsDevice.Clear(Color.Black);
			base.Draw(gameTime);
		}
	}
}