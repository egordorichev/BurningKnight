using BurningKnight.core.entity.item;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.secret {
	public class GoldSecretRoom : SecretRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(70)) {
				if (Random.Chance(50)) {
					Painter.Fill(Level, this, 2, Terrain.RandomFloor());
				} else {
					Painter.FillEllipse(Level, this, 2, Terrain.RandomFloor());
				}

			} 

			if (Random.Chance(70)) {
				bool Wall = Random.Chance(50);

				if (Random.Chance(50)) {
					Painter.FillEllipse(Level, this, 3, Wall ? Terrain.WALL : Terrain.CHASM);
				} else {
					Painter.Fill(Level, this, 3, Wall ? Terrain.WALL : Terrain.CHASM);
				}


				if (Random.Chance(70)) {
					if (Random.Chance(50)) {
						Painter.FillEllipse(Level, this, 4, !Wall ? Terrain.WALL : Terrain.CHASM);
					} else {
						Painter.Fill(Level, this, 4, !Wall ? Terrain.WALL : Terrain.CHASM);
					}

				} 
			} 

			for (int I = 0; I < Random.NewInt(3, 10); I++) {
				Point Point = this.GetRandomFreeCell();
				ItemHolder Holder = new ItemHolder(new Gold());
				Holder.X = Point.X * 16 + 3;
				Holder.Y = Point.Y * 16;
				Holder.GetItem().Generate();
				Dungeon.Area.Add(Holder);
				LevelSave.Add(Holder);
			}

			this.AddEnemies();
		}
	}
}
