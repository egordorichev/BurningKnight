using BurningKnight.entity.level.painters;

namespace BurningKnight.entity.level.rooms.regular {
	public class RectFloorRoom : RegularRoom {
		public override void Paint(Level Level) {
			base.Paint(Level);
			var F1 = Terrain.RandomFloor();
			var F2 = Terrain.RandomFloor();

			while (F2 == F1) F2 = Terrain.RandomFloor();

			for (var I = 1; I < GetWidth() / 2; I++) Painter.Fill(Level, this, I, I % 2 == 0 ? F1 : F2);
		}
	}
}