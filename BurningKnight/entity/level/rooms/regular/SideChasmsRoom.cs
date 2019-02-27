using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class SideChasmsRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.REGULAR);

			Painter.Fill(Level, this, Terrain.WALL);
			Painter.Fill(Level, this, 1, Terrain.CHASM);
			var M = Random.NewInt(2, 4);
			Painter.Fill(Level, this, M, Terrain.RandomFloor());
			PaintTunnel(Level, Terrain.RandomFloor());
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