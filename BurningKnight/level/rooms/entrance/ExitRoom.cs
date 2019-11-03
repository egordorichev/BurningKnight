using BurningKnight.level.entities;
using BurningKnight.level.tile;
using BurningKnight.state;
using Microsoft.Xna.Framework;

namespace BurningKnight.level.rooms.entrance {
	public class ExitRoom : RoomDef {
		public override int GetMinConnections(Connection Side) {
			if (Side == Connection.All) return 1;
			return 0;
		}

		public override int GetMaxConnections(Connection Side) {
			if (Side == Connection.All) return 16;
			return 4;
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

		public override void Paint(Level level) {
			base.Paint(level);
			
			var prop = new Exit {
				To = Run.Depth + 1
			};

			var where = GetCenter();
			
			Painter.Fill(level, where.X - 1, where.Y - 1, 3, 3, Tiles.RandomFloor());

			level.Area.Add(prop);
			prop.Center = (where * 16 + new Vector2(8));
		}
	}
}