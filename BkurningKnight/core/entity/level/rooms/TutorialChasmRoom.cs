using BurningKnight.core.entity.level.painters;
using BurningKnight.core.entity.level.rooms.regular;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms {
	public class TutorialChasmRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			Painter.DrawLine(Level, new Point(Left + GetWidth() / 2, Bottom - 1), new Point(Left + GetWidth() / 2, Top + 1), Terrain.CHASM);
		}
	}
}
