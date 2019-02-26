using BurningKnight.entity.level;

namespace BurningKnight.debug {
	public class PassableCommand : ConsoleCommand {
		public PassableCommand() {
			_Init();
		}

		protected void _Init() {
			{
				Name = "/pas";
				ShortName = "/p";
			}
		}

		public override void Run(Console Console, string[] Args) {
			Level.RENDER_PASSABLE = !Level.RENDER_PASSABLE;
		}
	}
}