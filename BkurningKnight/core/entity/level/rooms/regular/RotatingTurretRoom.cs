using BurningKnight.core.entity.level.painters;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.entity.trap;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class RotatingTurretRoom : TrapRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			Painter.Fill(Level, this, 1, Random.Chance(50) ? Terrain.FLOOR_A : Terrain.FLOOR_B);

			if (Random.Chance(50)) {
				Painter.FillEllipse(Level, this, 1, Random.Chance(50) ? Terrain.FLOOR_A : Terrain.FLOOR_B);
			} 

			Point Center = this.GetCenter();
			Turret Turret = Random.Chance(50) ? new FourSideRotatingTurret() : new RotatingTurret();
			Turret.X = Center.X * 16;
			Turret.Y = Center.Y * 16;
			Dungeon.Area.Add(Turret);
			LevelSave.Add(Turret);
		}

		public override int GetMinHeight() {
			return 8;
		}

		public override int GetMinWidth() {
			return 8;
		}
	}
}
