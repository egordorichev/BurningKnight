using BurningKnight.entity.creature.npc;
using BurningKnight.entity.door;
using BurningKnight.level.entities;
using BurningKnight.level.entities.decor;
using BurningKnight.level.tile;
using BurningKnight.level.walls;
using BurningKnight.save;
using BurningKnight.state;
using BurningKnight.util.geometry;
using Lens.entity;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.entrance {
	public class EntranceRoom : RoomDef {
		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.All) return 1;
			return 0;
		}

		public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.All) return 16;
			return 4;
		}

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

			var t = Tiles.RandomFloor();
			
			if (Run.Depth == Run.ContentEndDepth) {
				var om = new OldMan();
				level.Area.Add(om);
				om.Center = ((GetRandomDoorFreeCell() ?? GetTileCenter()) * 16 + new Dot(8)).ToVector();
			}
			
			Painter.Call(level, this, 1, (x, y) => {
				if (level.Get(x, y).Matches(Tile.SpikeTmp, Tile.SensingSpikeTmp, Tile.Chasm, Tile.Lava)) {
					level.Set(x, y, t);
				}
			});
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