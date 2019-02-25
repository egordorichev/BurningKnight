namespace BurningKnight.debug {
	public class ZoomCommand : ConsoleCommand {
		public ZoomCommand() {
			_Init();
		}

		protected void _Init() {
			{
				Name = "/zoom";
				ShortName = "/z";
			}
		}

		public override void Run(Console Console, string Args) {
			if (Args.Length == 0) return;

			float Zoom = Math.Max(0, Float.ValueOf(Args[0]));
			Gdx.Graphics.SetWindowedMode((int) (Display.GAME_WIDTH * Zoom), (int) (Display.GAME_HEIGHT * Zoom));
		}
	}
}