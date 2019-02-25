using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.entity.trap;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class SpikeTrapRoom : TrapRoom {
		public override Void Paint(Level Level) {
			byte F = Terrain.RandomFloor();
			Painter.Fill(Level, this, Terrain.WALL);

			if (Random.Chance(50)) {
				Painter.Fill(Level, this, 1, Terrain.CHASM);
			} 

			Painter.Fill(Level, new Rect(this.Left + 1, this.Top + 1, this.Left + 4, this.Bottom), F);
			Painter.Fill(Level, new Rect(this.Right - 3, this.Top + 1, this.Right, this.Bottom), F);
			int Y = this.Top + Random.NewInt(2, this.GetHeight() - 2);
			Painter.DrawLine(Level, new Point(this.Left + 1, Y), new Point(this.Right - 1, Y), F);
			Painter.DrawLine(Level, new Point(this.Left + 1, Y - 1), new Point(this.Right - 1, Y - 1), F);
			Painter.DrawLine(Level, new Point(this.Left + 1, Y + 1), new Point(this.Right - 1, Y + 1), F);
			RollingSpike Spike = new RollingSpike();
			Spike.X = (this.Left + 1) * 16 + 1;
			Spike.Y = Y * 16 - 1;
			Spike.Velocity = new Point(20f, 0);
			Dungeon.Area.Add(Spike);
			LevelSave.Add(Spike);

			foreach (LDoor Door in Connected.Values()) {
				Door.SetType(LDoor.Type.REGULAR);
			}
		}

		public override bool CanConnect(Point P) {
			if (P.X != this.Left && P.X != this.Right) {
				return false;
			} 

			return base.CanConnect(P);
		}

		public override int GetMinWidth() {
			return 10;
		}
	}
}
