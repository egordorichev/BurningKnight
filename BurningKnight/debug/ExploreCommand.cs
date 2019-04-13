namespace BurningKnight.debug {
	public class ExploreCommand : ConsoleCommand {
		public ExploreCommand() {
			Name = "explore";
			ShortName = "ex";
		}
		
		public override void Run(Console Console, string[] Args) {
			var level = state.Run.Level;

			for (var i = 0; i < level.Explored.Length; i++) {
				level.Explored[i] = true;
			}
		}
	}
}