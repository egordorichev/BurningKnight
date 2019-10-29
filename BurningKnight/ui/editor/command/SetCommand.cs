using BurningKnight.level;
using BurningKnight.level.tile;

namespace BurningKnight.ui.editor.command {
	public class SetCommand : Command {
		public Tile Tile;
		public int X;
		public int Y;

		private Tile before;
		private Tile beforeLiquid;
		
		public void Do(Level level) {
			var index = level.ToIndex(X, Y);

			before = level.Get(index);
			beforeLiquid = level.Get(index, true);
			
			level.Liquid[index] = 0;

			if (Tile.Matches(TileFlags.LiquidLayer) && (before.Matches(TileFlags.Solid) || before.Matches(TileFlags.Hole))) {
				level.Tiles[index] = (byte) Tiles.RandomFloor();
			}
			
			level.Set(index, Tile);
			level.UpdateTile(X, Y);
			level.ReCreateBodyChunk(X, Y);
		}

		public void Undo(Level level) {
			level.Set(X, Y, before);
			level.Set(X, Y, beforeLiquid);
			level.UpdateTile(X, Y);
			level.ReCreateBodyChunk(X, Y);
		}
	}
}