using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.entity.level.rooms.regular;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms {
	public class PrebossRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);
			Painter.Fill(Level, this, 1, Terrain.FLOOR_D);

			if (Random.Chance(50)) {
				var Fl = Random.Chance(50) ? Terrain.CHASM : Terrain.WALL;
				Painter.Fill(Level, this, 2, Fl);
				Painter.Fill(Level, this, 2 + Random.NewInt(1, 4), Terrain.FLOOR_D);
			}

			if (GetWidth() > 4 && GetHeight() > 4 && Random.Chance(50)) {
				PaintTunnel(Level, Terrain.FLOOR_D, true);
				PaintTunnel(Level, Terrain.RandomFloor());
			}
			else {
				PaintTunnel(Level, Terrain.FLOOR_D);
			}


			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.REGULAR);
		}

		protected override Point GetDoorCenter() {
			return GetCenter();
		}

		public override int GetMinWidth() {
			return 7;
		}

		public override int GetMinHeight() {
			return 7;
		}

		public override int GetMaxWidth() {
			return 8;
		}

		public override int GetMaxHeight() {
			return 8;
		}
	}
}