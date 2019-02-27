using BurningKnight.entity.level.entities;
using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.entity.level.rooms.regular;
using BurningKnight.entity.level.save;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.entrance {
	public class BossEntranceRoomDef : RegularRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(50))
				Painter.Fill(Level, this, 1, Terrain.RandomFloor());
			else
				Painter.FillEllipse(Level, this, 1, Terrain.RandomFloor());


			PaintTunnel(Level, Terrain.RandomFloor());
			Place(Level, GetCenter());

			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.LEVEL_LOCKED);
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

		public override bool CanConnect(RoomDef R) {
			return R is EntranceRoomDef && base.CanConnect(R);
		}

		protected override Point GetDoorCenter() {
			return GetCenter();
		}

		protected void Place(Level Level, Point Point) {
			var Exit = new Portal();
			Exit.X = Point.X * 16;
			Exit.Y = Point.Y * 16 - 8;
			Level.Set((int) Point.X, (int) Point.Y, Terrain.PORTAL);
			LevelSave.Add(Exit);
			Dungeon.Area.Add(Exit);
		}
	}
}