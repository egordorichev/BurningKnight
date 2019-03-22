using BurningKnight.entity.level;

namespace BurningKnight.entity.editor.command {
	public class SetCommand : Command {
		public Tile Tile;
		public int X;
		public int Y;

		private Tile before;
		
		public void Do(Level level) {
			before = level.Get(X, Y, Tile.Matches(TileFlags.LiquidLayer));
			level.Set(X, Y, Tile);
			level.UpdateTile(X, Y);
		}

		public void Undo(Level level) {
			level.Set(X, Y, before);
			level.UpdateTile(X, Y);
		}
	}
}