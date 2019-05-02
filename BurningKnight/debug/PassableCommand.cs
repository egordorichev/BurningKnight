using BurningKnight.level;

namespace BurningKnight.debug {
	public class PassableCommand : ConsoleCommand {
		public PassableCommand() {
			Name = "passable";
			ShortName = "p";
		}
		
		public override void Run(Console Console, string[] Args) {
			Level.RenderPassable = !Level.RenderPassable;
		}
	}
}