using BurningKnight.core.physics;

namespace BurningKnight.core.debug {
	public class DebugCommand : ConsoleCommand {
		protected void _Init() {
			{
				Name = "/debug";
				ShortName = "/d";
			}
		}

		public override Void Run(Console Console, string Args) {
			World.DRAW_DEBUG = !World.DRAW_DEBUG;
		}

		public DebugCommand() {
			_Init();
		}
	}
}
