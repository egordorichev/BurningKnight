using BurningKnight.entity.level.entities;
using BurningKnight.entity.level.features;
using BurningKnight.entity.level.painters;
using BurningKnight.entity.level.save;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.entrance {
	public class EntranceRoom : LadderRoom {
		public bool Exit;

		public override bool CanConnect(Room R) {
			return base.CanConnect(R) && !(R is EntranceRoom);
		}

		public override void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(50))
				Painter.Fill(Level, this, 2, Terrain.RandomFloor());
			else
				Painter.FillEllipse(Level, this, 2, Terrain.RandomFloor());


			Place(Level, GetCenter());
		}

		protected void Place(Level Level, Point Point) {
			if (this.Exit) {
				var Exit = new Portal();
				Exit.X = Point.X * 16;
				Exit.Y = Point.Y * 16;
				LevelSave.Add(Exit);
				Dungeon.Area.Add(Exit);
			}
			else {
				var Entrance = new Entrance();
				Entrance.X = Point.X * 16 + 1;
				Entrance.Y = Point.Y * 16 - 6;
				LevelSave.Add(Entrance);
				Dungeon.Area.Add(Entrance);
			}


			foreach (LDoor Door in Connected.Values()) Door.SetType(LDoor.Type.ENEMY);
		}
	}
}