using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.entrance {
	public class CircleEntranceRoom : EntranceRoom {
		public override void Paint(Level Level) {
			var F = Terrain.RandomFloor();
			Painter.Fill(Level, this, Terrain.WALL);
			Painter.FillEllipse(Level, this, 1, F);
			Painter.FillEllipse(Level, this, 2, Terrain.RandomFloor());
			PaintTunnel(Level, Terrain.RandomFloor());
			Place(Level, GetCenter());

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.REGULAR);
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