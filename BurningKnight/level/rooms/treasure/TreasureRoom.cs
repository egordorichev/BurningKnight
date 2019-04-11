using BurningKnight.entity.chest;
using BurningKnight.level.rooms.special;
using BurningKnight.level.tile;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.treasure {
	public class TreasureRoom : SpecialRoom {
		public override void Paint(Level level) {
			PlaceChest(level, GetCenter());
			
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Regular;
			}
		}

		public override void PaintFloor(Level level) {
			Painter.Fill(level, this, Tiles.RandomWall());
			Painter.Fill(level, this, 1, Tile.FloorD);	
		}

		protected void PlaceChest(Level level, Vector2 where) {
			var chest = new LockedChest();

			level.Area.Add(chest);

			chest.Center = where * 16;
			chest.GenerateLoot();
		}
	}
}