namespace BurningKnight.debug {
	public class TileCommand : ConsoleCommand {
		public TileCommand() {
			Name = "tile";
			ShortName = "t";
		}
		
		public override void Run(Console Console, string[] Args) {
			state.Run.Level?.TileUp(true);
		}
	}
}