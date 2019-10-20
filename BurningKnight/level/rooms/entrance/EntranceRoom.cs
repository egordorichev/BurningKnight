using BurningKnight.entity.creature.npc;
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

			var t = Tiles.RandomFloor();
			
			Painter.Call(level, this, 1, (x, y) => {
				if (level.Get(x, y).Matches(Tile.SpikeTmp, Tile.SensingSpikeTmp, Tile.Chasm, Tile.Lava)) {
					level.Set(x, y, t);
				}
			});
		}

		protected virtual void Place(Level level, Dot where) {
			var prop = Exit ? (Entity) new Exit {
				To = Run.Depth + 1
			} : new Entrance {
				To = -1
			};

			level.Area.Add(prop);

			var t = level.Get((int) where.X, (int) where.Y);

			if (Random.Chance(20) || !t.Matches(TileFlags.Passable)) {
				where = GetRandomDoorFreeCell() ?? GetTileCenter();
			}
			
			prop.Center = (where * 16 + new Dot(8)).ToVector();

			var torch = new Torch();
			level.Area.Add(torch);

			torch.Center = ((GetRandomDoorFreeCell() ?? GetTileCenter()) * 16 + new Dot(8)).ToVector();

			if ((Run.Depth == Run.ContentEndDepth) || (Run.Depth == 1 && GlobalSave.IsFalse("control_roll"))) {
				var om = new OldMan();
				level.Area.Add(om);
				om.Center = ((GetRandomDoorFreeCell() ?? GetTileCenter()) * 16 + new Dot(8)).ToVector();
			}

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