using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.entity.level.rooms.regular;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms {
	public class PrebossRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			Painter.Fill(Level, this, 1, Terrain.FLOOR_D);

			if (Random.Chance(50)) {
				byte Fl = Random.Chance(50) ? Terrain.CHASM : Terrain.WALL;
				Painter.Fill(Level, this, 2, Fl);
				Painter.Fill(Level, this, 2 + Random.NewInt(1, 4), Terrain.FLOOR_D);
			} 

			if (this.GetWidth() > 4 && this.GetHeight() > 4 && Random.Chance(50)) {
				this.PaintTunnel(Level, Terrain.FLOOR_D, true);
				this.PaintTunnel(Level, Terrain.RandomFloor());
			} else {
				this.PaintTunnel(Level, Terrain.FLOOR_D);
			}


			foreach (LDoor Door in this.Connected.Values()) {
				Door.SetType(LDoor.Type.REGULAR);
			}
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
