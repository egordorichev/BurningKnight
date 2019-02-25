using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class LetterRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			byte F = Random.Chance(50) ? Terrain.WALL : Terrain.CHASM;
			float R = Random.NewFloat();

			if (R < 0.25f) {
				Painter.DrawLine(Level, new Point(this.Left + 2, this.Top + 2), new Point(this.Right - 2, this.Bottom - 2), F);
				Painter.DrawLine(Level, new Point(this.Left + 2, this.Bottom - 2), new Point(this.Right - 2, this.Top + 2), F);
			} else if (R < 0.5f) {
				Painter.DrawLine(Level, new Point(this.Left + this.GetWidth() / 2, this.Bottom - 2), new Point(this.Left + this.GetWidth() / 2, this.Top + 2), F);
			} else if (R < 0.75f) {
				Painter.DrawLine(Level, new Point(this.Left + this.GetWidth() / 2, this.Bottom - 2), new Point(this.Left + this.GetWidth() / 2, this.Top + 2), F);
				Painter.DrawLine(Level, new Point(this.Left + 2, this.Bottom - 2), new Point(this.Right - 2, this.Bottom - 2), F);
			} else {
				Painter.DrawLine(Level, new Point(this.Left + 2, this.Bottom - 2), new Point(this.Left + 2, this.Top + 2), F);
				Painter.DrawLine(Level, new Point(this.Left + 2, this.Top + 2), new Point(this.Right - 2, this.Top + 2), F);
				Painter.DrawLine(Level, new Point(this.Left + 2, this.Top + GetHeight() / 2), new Point(this.Right - 2, this.Top + GetHeight() / 2), F);
				Painter.DrawLine(Level, new Point(this.Left + 2, this.Bottom - 2), new Point(this.Right - 2, this.Bottom - 2), F);
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
