using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.entity.level.save;
using BurningKnight.entity.trap;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.regular {
	public class VerticalSpikeTrapRoomDef : TrapRoomDef {
		public override void Paint(Level Level) {
			var F = Terrain.RandomFloor();
			Painter.Fill(Level, this, Terrain.WALL);

			if (Random.Chance(50)) Painter.Fill(Level, this, 1, Terrain.CHASM);

			Painter.Fill(Level, new Rect(Left + 1, Top + 1, Right, Top + 4), F);
			Painter.Fill(Level, new Rect(Left + 1, Bottom - 3, Right, Bottom), F);
			var Y = Left + Random.NewInt(2, GetWidth() - 2);
			Painter.DrawLine(Level, new Point(Y, Top + 1), new Point(Y, Bottom - 1), F);
			Painter.DrawLine(Level, new Point(Y + 1, Top + 1), new Point(Y + 1, Bottom - 1), F);
			Painter.DrawLine(Level, new Point(Y - 1, Top + 1), new Point(Y - 1, Bottom - 1), F);
			var Spike = new RollingSpike();
			Spike.X = Y * 16 + 1;
			Spike.Y = (Top + 1) * 16 + 1;
			Spike.Velocity = new Point(0f, 20f);
			Dungeon.Area.Add(Spike);
			LevelSave.Add(Spike);

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.REGULAR);
		}

		public override bool CanConnect(Point P) {
			if (P.Y != Top && P.Y != Bottom) return false;

			return base.CanConnect(P);
		}

		public override int GetMinHeight() {
			return 10;
		}
	}
}