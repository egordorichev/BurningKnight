using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class LetterRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);
			var F = Random.Chance(50) ? Terrain.WALL : Terrain.CHASM;
			var R = Random.NewFloat();

			if (R < 0.25f) {
				Painter.DrawLine(Level, new Point(Left + 2, Top + 2), new Point(Right - 2, Bottom - 2), F);
				Painter.DrawLine(Level, new Point(Left + 2, Bottom - 2), new Point(Right - 2, Top + 2), F);
			}
			else if (R < 0.5f) {
				Painter.DrawLine(Level, new Point(Left + GetWidth() / 2, Bottom - 2), new Point(Left + GetWidth() / 2, Top + 2), F);
			}
			else if (R < 0.75f) {
				Painter.DrawLine(Level, new Point(Left + GetWidth() / 2, Bottom - 2), new Point(Left + GetWidth() / 2, Top + 2), F);
				Painter.DrawLine(Level, new Point(Left + 2, Bottom - 2), new Point(Right - 2, Bottom - 2), F);
			}
			else {
				Painter.DrawLine(Level, new Point(Left + 2, Bottom - 2), new Point(Left + 2, Top + 2), F);
				Painter.DrawLine(Level, new Point(Left + 2, Top + 2), new Point(Right - 2, Top + 2), F);
				Painter.DrawLine(Level, new Point(Left + 2, Top + GetHeight() / 2), new Point(Right - 2, Top + GetHeight() / 2), F);
				Painter.DrawLine(Level, new Point(Left + 2, Bottom - 2), new Point(Right - 2, Bottom - 2), F);
			}
		}

		public override int GetMinHeight() {
			return 8;
		}

		public override int GetMinWidth() {
			return 8;
		}
	}
}