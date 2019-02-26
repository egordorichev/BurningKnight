using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.entity.level.save;
using BurningKnight.entity.trap;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class SpikeTrapRoom : TrapRoom {
		public override void Paint(Level Level) {
			var F = Terrain.RandomFloor();
			Painter.Fill(Level, this, Terrain.WALL);

			if (Random.Chance(50)) Painter.Fill(Level, this, 1, Terrain.CHASM);

			Painter.Fill(Level, new Rect(Left + 1, Top + 1, Left + 4, Bottom), F);
			Painter.Fill(Level, new Rect(Right - 3, Top + 1, Right, Bottom), F);
			var Y = Top + Random.NewInt(2, GetHeight() - 2);
			Painter.DrawLine(Level, new Point(Left + 1, Y), new Point(Right - 1, Y), F);
			Painter.DrawLine(Level, new Point(Left + 1, Y - 1), new Point(Right - 1, Y - 1), F);
			Painter.DrawLine(Level, new Point(Left + 1, Y + 1), new Point(Right - 1, Y + 1), F);
			var Spike = new RollingSpike();
			Spike.X = (Left + 1) * 16 + 1;
			Spike.Y = Y * 16 - 1;
			Spike.Velocity = new Point(20f, 0);
			Dungeon.Area.Add(Spike);
			LevelSave.Add(Spike);

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.REGULAR);
		}

		public override bool CanConnect(Point P) {
			if (P.X != Left && P.X != Right) return false;

			return base.CanConnect(P);
		}

		public override int GetMinWidth() {
			return 10;
		}
	}
}