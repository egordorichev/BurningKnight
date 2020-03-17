using System.Collections.Generic;
using BurningKnight.level.entities;
using BurningKnight.level.rooms;
using BurningKnight.level.tile;
using BurningKnight.util.geometry;
using Lens.util.math;

namespace BurningKnight.level.walls.library {
	public class LibraryWallPainter : WallPainter {
		private List<string> unusedTeleporters = new List<string>();

		public override void Paint(Level level, RoomDef room, Rect inside) {
			base.Paint(level, room, inside);
			unusedTeleporters.AddRange(Teleporter.Ids);
		}

		public void PlaceTeleporter(Level level, Dot a, Dot b) {
			var index = Rnd.Int(unusedTeleporters.Count);
			var id = unusedTeleporters[index];

			unusedTeleporters.RemoveAt(index);

			var at = new Teleporter {
				Id = id
			};
			
			level.Area.Add(at);
			at.Position = a * 16;
			
			Painter.Set(level, a, Tiles.RandomFloor());
			
			var bt = new Teleporter {
				Id = id
			};
			
			level.Area.Add(bt);
			bt.Position = b * 16;
			
			Painter.Set(level, b, Tiles.RandomFloor());
		}
	}
}