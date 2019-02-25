using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class SideChasmsRoom : RegularRoom {
		public override Void Paint(Level Level) {
			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.REGULAR);
			}

			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.CHASM);
			int M = Random.NewInt(2, 4);
			Painter.Fill(Level, this, M, Terrain.RandomFloor());
			this.PaintTunnel(Level, Terrain.RandomFloor());
		}

		protected override Point GetDoorCenter() {
			return GetCenter();
		}

		public override int GetMinWidth() {
			return 9;
		}

		public override int GetMinHeight() {
			return 9;
		}
	}
}
