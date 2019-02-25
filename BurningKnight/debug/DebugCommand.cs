using BurningKnight.physics;

namespace BurningKnight.debug {
	public class DebugCommand : ConsoleCommand {
		public DebugCommand() {
			_Init();
		}

		protected void _Init() {
			{
				Name = "/debug";
				ShortName = "/d";
			}
		}

		public override void Run(Console Console, string[] Args) {
			World.DRAW_DEBUG = !World.DRAW_DEBUG;
		}
	}
}