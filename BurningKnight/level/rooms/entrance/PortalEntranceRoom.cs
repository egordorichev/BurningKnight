using BurningKnight.entity.creature.npc;
using BurningKnight.entity.door;
using BurningKnight.level.entities;
using BurningKnight.level.tile;
using BurningKnight.state;
using BurningKnight.util.geometry;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.entrance {
	public class PortalEntranceRoom : EntranceRoom {
		public override void Paint(Level level) {
			// WallRegistry.Paint(level, this, EntranceWallPool.Instance);

			var w = GetTileCenter();
			var d = w + new Dot(0, -2);
			
			Painter.Fill(level, d.X - 1, d.Y - 1, 3, 3, Tile.WallA);
			Painter.Set(level, d.X, d.Y, Tiles.RandomFloor());
			Painter.Set(level, d.X, d.Y + 1, Tiles.RandomFloor());

			var door = new LevelDoor();
			level.Area.Add(door);

			door.X = d.X * 16;
			door.Y = d.Y * 16 + 16;

			PlaceExit(level, d);
			PlaceEntrance(level, w + new Dot(0, 1));

			MakeSafe(level);
		}

		private void PlaceExit(Level level, Dot where) {
			var prop = new Exit {
				To = Run.Depth + 1
			};

			level.Area.Add(prop);
			prop.Center = (where * 16 + new Vector2(8));
		}

		private void PlaceEntrance(Level level, Dot where) {
			var prop = new Entrance {
				To = -1
			};

			level.Area.Add(prop);
			prop.Center = (where * 16 + new Vector2(8));
		}

		public override int GetMinWidth() {
			return 7;
		}

		public override int GetMinHeight() {
			return 10;
		}

		public override int GetMaxWidth() {
			return 12;
		}

		public override int GetMaxHeight() {
			return 12;
		}
	}
}