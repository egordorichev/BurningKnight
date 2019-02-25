using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.entrance {
	public class CircleEntranceRoom : EntranceRoom {
		public override Void Paint(Level Level) {
			byte F = Terrain.RandomFloor();
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.FillEllipse(Level, this, 1, F);
			Painter.FillEllipse(Level, this, 2, Terrain.RandomFloor());
			PaintTunnel(Level, Terrain.RandomFloor());
			this.Place(Level, this.GetCenter());

			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.REGULAR);
			}
		}

		protected override Point GetDoorCenter() {
			return GetCenter();
		}

		public override int GetMinWidth() {
			return 5;
		}

		public override int GetMinHeight() {
			return 5;
		}

		public override int GetMaxWidth() {
			return 10;
		}

		public override int GetMaxHeight() {
			return 10;
		}
	}
}
