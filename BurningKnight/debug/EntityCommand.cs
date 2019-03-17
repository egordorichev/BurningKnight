using Lens.game;

namespace BurningKnight.debug {
	public class EntityCommand : ConsoleCommand {
		public EntityCommand() {
			Name = "entity";
			ShortName = "e";
		}
		
		public override void Run(Console Console, string[] Args) {
			GameState.RenderDebug = !GameState.RenderDebug;
		}
	}
}