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
			var chest = new LockedChest {
				X = where.X * 16, 
				Y = where.Y * 16
			};

			level.Area.Add(chest);
			chest.GenerateLoot();
		}
	}
}