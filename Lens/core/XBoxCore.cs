namespace Lens.Core {
	public class XBoxCore : ConsoleCore  {
		public override void Init(int width, int height, bool fullscreen) {
			base.Init(width, height, fullscreen);
			
			Graphics.PreferredBackBufferWidth = 1920;
			Graphics.PreferredBackBufferHeight = 1080;
		}
	}
}