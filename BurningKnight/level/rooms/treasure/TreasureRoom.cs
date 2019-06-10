using BurningKnight.entity.chest;
using BurningKnight.level.rooms.special;
using BurningKnight.level.tile;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.treasure {
	public class TreasureRoom : SpecialRoom {
		public override void Paint(Level level) {
			PlaceChest(level, GetCenter());
		}

		public override void PaintFloor(Level level) {
			Painter.Fill(level, this, Tiles.RandomWall());
			Painter.Fill(level, this, 1, Tile.FloorD);	
		}

		protected void PlaceChest(Level level, Vector2 where) {
			var chest = Random.Chance(30) ? new Chest() : new LockedChest();

			level.Area.Add(chest);

			chest.Center = where * 16;
			chest.GenerateLoot();
		}

		public override bool ShouldSpawnMobs() {
			return Random.Chance(10);
		}
	}
}