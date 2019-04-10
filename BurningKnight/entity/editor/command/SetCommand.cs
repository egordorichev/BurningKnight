using BurningKnight.level;
using BurningKnight.level.tile;

namespace BurningKnight.entity.editor.command {
	public class SetCommand : Command {
		public Tile Tile;
		public int X;
		public int Y;

		private Tile before;
		private bool liquid;
		
		public void Do(Level level) {
			var index = level.ToIndex(X, Y);
			liquid = level.Liquid[index] != 0;
			
			before = liquid ? level.Get(index, true) : level.Get(index);
			
			level.Liquid[index] = 0;

			if (Tile.Matches(TileFlags.LiquidLayer) && (before.Matches(TileFlags.Solid) || before.Matches(TileFlags.Hole))) {
				level.Tiles[index] = (byte) Tiles.RandomFloor();
			}
			
			level.Set(index, Tile);
			level.UpdateTile(X, Y);
		}

		public void Undo(Level level) {
			level.Set(X, Y, before);
			level.UpdateTile(X, Y);
		}
	}
}