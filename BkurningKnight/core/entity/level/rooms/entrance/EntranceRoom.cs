using BurningKnight.core.entity.level.entities;
using BurningKnight.core.entity.level.features;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.entrance {
	public class EntranceRoom : LadderRoom {
		public bool Exit;

		public override bool CanConnect(Room R) {
			return base.CanConnect(R) && !(R is EntranceRoom);
		}

		public override Void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(50)) {
				Painter.Fill(Level, this, 2, Terrain.RandomFloor());
			} else {
				Painter.FillEllipse(Level, this, 2, Terrain.RandomFloor());
			}


			this.Place(Level, this.GetCenter());
		}

		protected Void Place(Level Level, Point Point) {
			if (this.Exit) {
				Portal Exit = new Portal();
				Exit.X = Point.X * 16;
				Exit.Y = Point.Y * 16;
				LevelSave.Add(Exit);
				Dungeon.Area.Add(Exit);
			} else {
				Entrance Entrance = new Entrance();
				Entrance.X = Point.X * 16 + 1;
				Entrance.Y = Point.Y * 16 - 6;
				LevelSave.Add(Entrance);
				Dungeon.Area.Add(Entrance);
			}


			foreach (LDoor Door in Connected.Values()) {
				Door.SetType(LDoor.Type.ENEMY);
			}
		}
	}
}
