using BurningKnight.core.entity.level.entities;
using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.entity.level.rooms.regular;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.entrance {
	public class BossEntranceRoom : RegularRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(50)) {
				Painter.Fill(Level, this, 1, Terrain.RandomFloor());
			} else {
				Painter.FillEllipse(Level, this, 1, Terrain.RandomFloor());
			}


			PaintTunnel(Level, Terrain.RandomFloor());
			this.Place(Level, this.GetCenter());

			foreach (LDoor Door in Connected.Values()) {
				Door.SetType(LDoor.Type.LEVEL_LOCKED);
			}
		}

		public override int GetMinWidth() {
			return 5;
		}

		public override int GetMinHeight() {
			return 5;
		}

		public override int GetMaxHeight() {
			return 6;
		}

		public override int GetMaxWidth() {
			return 6;
		}

		public override bool CanConnect(Room R) {
			return R is EntranceRoom && base.CanConnect(R);
		}

		protected override Point GetDoorCenter() {
			return GetCenter();
		}

		protected Void Place(Level Level, Point Point) {
			Portal Exit = new Portal();
			Exit.X = Point.X * 16;
			Exit.Y = Point.Y * 16 - 8;
			Level.Set((int) Point.X, (int) Point.Y, Terrain.PORTAL);
			LevelSave.Add(Exit);
			Dungeon.Area.Add(Exit);
		}
	}
}
