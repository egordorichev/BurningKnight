using BurningKnight.core.entity.level.painters;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class RectFloorRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			byte F1 = Terrain.RandomFloor();
			byte F2 = Terrain.RandomFloor();

			while (F2 == F1) {
				F2 = Terrain.RandomFloor();
			}

			for (int I = 1; I < this.GetWidth() / 2; I++) {
				Painter.Fill(Level, this, I, I % 2 == 0 ? F1 : F2);
			}
		}
	}
}
