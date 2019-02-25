namespace BurningKnight.core.debug {
	public class ZoomCommand : ConsoleCommand {
		protected void _Init() {
			{
				Name = "/zoom";
				ShortName = "/z";
			}
		}

		public override Void Run(Console Console, string Args) {
			if (Args.Length == 0) {
				return;
			} 

			float Zoom = Math.Max(0, Float.ValueOf(Args[0]));
			Gdx.Graphics.SetWindowedMode((int) (Display.GAME_WIDTH * Zoom), (int) (Display.GAME_HEIGHT * Zoom));
		}

		public ZoomCommand() {
			_Init();
		}
	}
}
