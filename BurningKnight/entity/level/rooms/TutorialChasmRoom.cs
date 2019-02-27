using BurningKnight.entity.level.painters;
using BurningKnight.entity.level.rooms.regular;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms {
	public class TutorialChasmRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);
			Painter.DrawLine(Level, new Point(Left + GetWidth() / 2, Bottom - 1), new Point(Left + GetWidth() / 2, Top + 1), Terrain.CHASM);
		}
	}
}