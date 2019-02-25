using BurningKnight.core.entity.creature.fx;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.secret {
	public class HeartRoom : SecretRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(70)) {
				if (Random.Chance(50)) {
					Painter.Fill(Level, this, 2, Terrain.RandomFloor());
				} else {
					Painter.FillEllipse(Level, this, 2, Terrain.RandomFloor());
				}

			} 

			for (int I = 0; I < Random.NewInt(3, 10); I++) {
				Point Point = this.GetRandomFreeCell();
				HeartFx Holder = new HeartFx();
				Holder.X = Point.X * 16 + 3;
				Holder.Y = Point.Y * 16;
				Dungeon.Area.Add(Holder);
				LevelSave.Add(Holder);
			}

			this.AddEnemies();
		}
	}
}
