using BurningKnight.entity.creature.fx;
using BurningKnight.entity.item;
using BurningKnight.entity.item.key;
using BurningKnight.entity.level.painters;
using BurningKnight.entity.level.save;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.secret {
	public class MixedSecretRoomDef : SecretRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);
			Painter.Fill(Level, this, 1, Terrain.FLOOR_D);
			Painter.FillEllipse(Level, this, 2, Terrain.RandomFloor());

			for (var I = 0; I < Random.NewInt(1, 4); I++) {
				var Point = GetRandomFreeCell();
				var Holder = new ItemHolder(new Bomb());
				Holder.X = Point.X * 16 + 3;
				Holder.Y = Point.Y * 16;
				Dungeon.Area.Add(Holder);
				LevelSave.Add(Holder);
			}

			for (var I = 0; I < Random.NewInt(1, 3); I++) {
				var Point = GetRandomFreeCell();
				var Holder = new ItemHolder(new KeyC());
				Holder.X = Point.X * 16 + 3;
				Holder.Y = Point.Y * 16;
				Dungeon.Area.Add(Holder);
				LevelSave.Add(Holder);
			}

			for (var I = 0; I < Random.NewInt(1, 3); I++) {
				var Point = GetRandomFreeCell();
				var Holder = new ItemHolder(new Gold());
				Holder.GetItem().Generate();
				Holder.X = Point.X * 16 + 3;
				Holder.Y = Point.Y * 16;
				Dungeon.Area.Add(Holder);
				LevelSave.Add(Holder);
			}

			for (var I = 0; I < Random.NewInt(1, 3); I++) {
				var Point = GetRandomFreeCell();
				var Holde = new HeartFx();
				Holde.X = Point.X * 16 + 3;
				Holde.Y = Point.Y * 16;
				Dungeon.Area.Add(Holde);
				LevelSave.Add(Holde);
			}

			AddEnemies();
		}
	}
}