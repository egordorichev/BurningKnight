using BurningKnight.level.tile;
using Lens.util.math;

namespace BurningKnight.level.rooms.connection {
	public class IceConnectionRoom : ConnectionRoom {
		public override void PaintFloor(Level level) {
			
		}

		public override void Paint(Level level) {
			var old = Painter.Clip;
			
			Painter.Clip = null;
			Painter.Fill(level, this, Tile.WallA);
			Painter.Clip = old;
			
			foreach (var door in Connected.Values) {
				var sx = Rnd.Int(3, 7);
				var sy = Rnd.Int(3, 7);
				Painter.FillEllipse(level, door.X - sx / 2, door.Y - sy / 2, sx, sy, Tiles.RandomFloor());
			}
		}

		public override void SetupDoors(Level level) {
			foreach (var door in Connected.Values) {
				door.Type = DoorPlaceholder.Variant.Empty;
			}
		}

		public override int GetMinWidth() {
			return 11;
		}

		public override int GetMaxWidth() {
			return 22;
		}

		public override int GetMinHeight() {
			return 11;
		}

		public override int GetMaxHeight() {
			return 22;
		}
	}
}