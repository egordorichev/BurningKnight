using BurningKnight.entity.level.painters;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class CornerRoom : RegularRoom {
		public override void Paint(Level Level) {
			base.Paint(Level);
			var F = Random.Chance(50) ? Terrain.WALL : Terrain.CHASM;
			Painter.Set(Level, new Point(Left + 1, Top + 1), F);
			Painter.Set(Level, new Point(Right - 1, Top + 1), F);
			Painter.Set(Level, new Point(Left + 1, Bottom - 1), F);
			Painter.Set(Level, new Point(Right - 1, Bottom - 1), F);
			Painter.Fill(Level, this, 2, Terrain.RandomFloor());
			Painter.FillEllipse(Level, this, Random.NewInt(2, 4), Terrain.RandomFloor());

			if (Random.Chance(50)) PaintTunnel(Level, Random.Chance(50) ? Terrain.FLOOR_D : Terrain.RandomFloor());
		}

		public override bool CanConnect(Point P) {
			if (P.X == Left + 1 && (P.Y == Top || P.Y == Bottom)) return false;

			if (P.X == Right - 1 && (P.Y == Top || P.Y == Bottom)) return false;

			if (P.Y == Top + 1 && (P.X == Left || P.X == Right)) return false;

			if (P.Y == Bottom - 1 && (P.X == Left || P.X == Right)) return false;

			return base.CanConnect(P);
		}
	}
}