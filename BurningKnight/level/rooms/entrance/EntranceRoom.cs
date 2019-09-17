using BurningKnight.level.entities;
using BurningKnight.level.entities.decor;
using BurningKnight.level.tile;
using BurningKnight.level.walls;
using BurningKnight.state;
using Lens.entity;
using Lens.util.math;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.entrance {
	public class EntranceRoom : RoomDef {
		public bool Exit;

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
			Place(level, GetTileCenter());
		}

		protected virtual void Place(Level level, Vector2 where) {
			var prop = Exit ? (Entity) new Exit {
				To = Run.Depth + 1
			} : new Entrance {
				To = -1
			};

			level.Area.Add(prop);

			var t = level.Get((int) where.X, (int) where.Y);

			if (Random.Chance(20) || !t.Matches(TileFlags.Passable)) {
				where = GetRandomFreeCell().GetValueOrDefault(GetTileCenter());
			}
			
			prop.Center = where * 16 + new Vector2(8);

			var torch = new Torch();
			level.Area.Add(torch);

			torch.Center = GetRandomFreeCell().GetValueOrDefault(GetTileCenter()) * 16 + new Vector2(8);
			
			
			/*
			var bk = new entity.creature.bk.BurningKnight();
			level.Area.Add(bk);
			bk.Center = where * 16;*/
		}

		public override int GetMinWidth() {
			return 7;
		}

		public override int GetMinHeight() {
			return 7;
		}

		public override int GetMaxWidth() {
			return 12;
		}

		public override int GetMaxHeight() {
			return 12;
		}
	}
}