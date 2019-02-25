using BurningKnight.core.entity.level;

namespace BurningKnight.core.debug {
	public class PassableCommand : ConsoleCommand {
		protected void _Init() {
			{
				Name = "/pas";
				ShortName = "/p";
			}
		}

		public override Void Run(Console Console, string Args) {
			Level.RENDER_PASSABLE = !Level.RENDER_PASSABLE;
		}

		public PassableCommand() {
			_Init();
		}
	}
}
