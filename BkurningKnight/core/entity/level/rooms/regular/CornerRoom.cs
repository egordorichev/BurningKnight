using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class CornerRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			byte F = Random.Chance(50) ? Terrain.WALL : Terrain.CHASM;
			Painter.Set(Level, new Point(this.Left + 1, this.Top + 1), F);
			Painter.Set(Level, new Point(this.Right - 1, this.Top + 1), F);
			Painter.Set(Level, new Point(this.Left + 1, this.Bottom - 1), F);
			Painter.Set(Level, new Point(this.Right - 1, this.Bottom - 1), F);
			Painter.Fill(Level, this, 2, Terrain.RandomFloor());
			Painter.FillEllipse(Level, this, Random.NewInt(2, 4), Terrain.RandomFloor());

			if (Random.Chance(50)) {
				PaintTunnel(Level, Random.Chance(50) ? Terrain.FLOOR_D : Terrain.RandomFloor());
			} 
		}

		public override bool CanConnect(Point P) {
			if (P.X == this.Left + 1 && (P.Y == this.Top || P.Y == this.Bottom)) {
				return false;
			} 

			if (P.X == this.Right - 1 && (P.Y == this.Top || P.Y == this.Bottom)) {
				return false;
			} 

			if (P.Y == this.Top + 1 && (P.X == this.Left || P.X == this.Right)) {
				return false;
			} 

			if (P.Y == this.Bottom - 1 && (P.X == this.Left || P.X == this.Right)) {
				return false;
			} 

			return base.CanConnect(P);
		}
	}
}
