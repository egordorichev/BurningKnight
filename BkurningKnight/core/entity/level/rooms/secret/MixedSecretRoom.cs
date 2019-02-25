using BurningKnight.core.entity.creature.fx;
using BurningKnight.core.entity.item;
using BurningKnight.core.entity.item.key;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.secret {
	public class MixedSecretRoom : SecretRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			Painter.Fill(Level, this, 1, Terrain.FLOOR_D);
			Painter.FillEllipse(Level, this, 2, Terrain.RandomFloor());

			for (int I = 0; I < Random.NewInt(1, 4); I++) {
				Point Point = this.GetRandomFreeCell();
				ItemHolder Holder = new ItemHolder(new Bomb());
				Holder.X = Point.X * 16 + 3;
				Holder.Y = Point.Y * 16;
				Dungeon.Area.Add(Holder);
				LevelSave.Add(Holder);
			}

			for (int I = 0; I < Random.NewInt(1, 3); I++) {
				Point Point = this.GetRandomFreeCell();
				ItemHolder Holder = new ItemHolder(new KeyC());
				Holder.X = Point.X * 16 + 3;
				Holder.Y = Point.Y * 16;
				Dungeon.Area.Add(Holder);
				LevelSave.Add(Holder);
			}

			for (int I = 0; I < Random.NewInt(1, 3); I++) {
				Point Point = this.GetRandomFreeCell();
				ItemHolder Holder = new ItemHolder(new Gold());
				Holder.GetItem().Generate();
				Holder.X = Point.X * 16 + 3;
				Holder.Y = Point.Y * 16;
				Dungeon.Area.Add(Holder);
				LevelSave.Add(Holder);
			}

			for (int I = 0; I < Random.NewInt(1, 3); I++) {
				Point Point = this.GetRandomFreeCell();
				HeartFx Holde = new HeartFx();
				Holde.X = Point.X * 16 + 3;
				Holde.Y = Point.Y * 16;
				Dungeon.Area.Add(Holde);
				LevelSave.Add(Holde);
			}

			this.AddEnemies();
		}
	}
}
