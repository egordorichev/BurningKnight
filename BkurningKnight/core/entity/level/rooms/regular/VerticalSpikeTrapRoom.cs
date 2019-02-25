using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.entity.trap;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.regular {
	public class VerticalSpikeTrapRoom : TrapRoom {
		public override Void Paint(Level Level) {
			byte F = Terrain.RandomFloor();
			Painter.Fill(Level, this, Terrain.WALL);

			if (Random.Chance(50)) {
				Painter.Fill(Level, this, 1, Terrain.CHASM);
			} 

			Painter.Fill(Level, new Rect(this.Left + 1, this.Top + 1, this.Right, this.Top + 4), F);
			Painter.Fill(Level, new Rect(this.Left + 1, this.Bottom - 3, this.Right, this.Bottom), F);
			int Y = this.Left + Random.NewInt(2, this.GetWidth() - 2);
			Painter.DrawLine(Level, new Point(Y, this.Top + 1), new Point(Y, this.Bottom - 1), F);
			Painter.DrawLine(Level, new Point(Y + 1, this.Top + 1), new Point(Y + 1, this.Bottom - 1), F);
			Painter.DrawLine(Level, new Point(Y - 1, this.Top + 1), new Point(Y - 1, this.Bottom - 1), F);
			RollingSpike Spike = new RollingSpike();
			Spike.X = Y * 16 + 1;
			Spike.Y = (this.Top + 1) * 16 + 1;
			Spike.Velocity = new Point(0f, 20f);
			Dungeon.Area.Add(Spike);
			LevelSave.Add(Spike);

			foreach (LDoor Door in Connected.Values()) {
				Door.SetType(LDoor.Type.REGULAR);
			}
		}

		public override bool CanConnect(Point P) {
			if (P.Y != this.Top && P.Y != this.Bottom) {
				return false;
			} 

			return base.CanConnect(P);
		}

		public override int GetMinHeight() {
			return 10;
		}
	}
}
