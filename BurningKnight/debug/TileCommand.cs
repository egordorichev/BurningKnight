namespace BurningKnight.debug {
	public class TileCommand : ConsoleCommand {
		public TileCommand() {
			Name = "tile";
			ShortName = "t";
		}
		
		public override void Run(Console Console, string[] Args) {
			var level = state.Run.Level;

			if (level != null) {
				// level.Resize(level.Width, level.Height);
				level.RefreshSurfaces();
				level.TileUp(true);
			}
		}
	}
}