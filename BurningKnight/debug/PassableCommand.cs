using BurningKnight.level;

namespace BurningKnight.debug {
	public class PassableCommand : ConsoleCommand {
		public override void Run(Console Console, string[] Args) {
			Level.RenderPassable = !Level.RenderPassable;
		}
	}
}