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
			WallRegistry.Paint(level, this, EntranceWallPool.Instance);
			
			var prop = new Entrance {
				To = Run.Depth + 1
			};

			var where = GetCenter();
			
			Painter.Fill(level, where.X - 1, where.Y - 1, 3, 3, Tiles.RandomFloor());

			level.Area.Add(prop);
			prop.Center = (where * 16 + new Vector2(8));

			MakeSafe(level);
		}

		protected void MakeSafe(Level level) {
			var t = Tiles.RandomFloor();
			
			Painter.Call(level, this, 1, (x, y) => {
				if (level.Get(x, y).Matches(Tile.SpikeTmp, Tile.SensingSpikeTmp, Tile.Chasm, Tile.Lava)) {
					level.Set(x, y, t);
				}
			});
			
			if (Run.Depth == Run.ContentEndDepth) {
				var om = new OldMan();
				level.Area.Add(om);
				om.Center = ((GetRandomDoorFreeCell() ?? GetTileCenter()) * 16 + new Dot(8)).ToVector();
			}
		}

		public override int GetMinWidth() {
			return 5;
		}

		public override int GetMinHeight() {
			return 5;
		}

		public override int GetMaxWidth() {
			return 11;
		}

		public override int GetMaxHeight() {
			return 11;
		}
	}
}