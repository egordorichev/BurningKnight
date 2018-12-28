namespace Lens.Core {
	public class SwitchCore : ConsoleCore {
		public override void Init(int width, int height, bool fullscreen) {
			base.Init(width, height, fullscreen);
			
			Graphics.PreferredBackBufferWidth = 1280;
			Graphics.PreferredBackBufferHeight = 720;
		}
	}
}