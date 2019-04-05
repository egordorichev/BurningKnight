using BurningKnight.entity.chest;
using BurningKnight.entity.level.rooms.special;
using Microsoft.Xna.Framework;

namespace BurningKnight.entity.level.rooms.treasure {
	public class TreasureRoom : SpecialRoom {
		public override void Paint(Level level) {
			base.Paint(level);
			PlaceChest(level, GetCenter());
		}

		public override void PaintFloor(Level level) {
			Painter.Fill(level, this, Tiles.RandomWall());
			Painter.Fill(level, this, 1, Tile.FloorD);	
		}

		protected void PlaceChest(Level level, Vector2 where) {
			var chest = new LockedChest(); // todo: chest pool and such
			
			chest.X = where.X * 16;
			chest.Y = where.Y * 16;

			level.Area.Add(chest);
		}

		protected override int ValidateWidth(int W) {
			return W % 2 == 0 ? W : W + 1;
		}

		protected override int ValidateHeight(int H) {
			return H % 2 == 0 ? H : H + 1;
		}
	}
}