using BurningKnight.entity.creature.fx;
using BurningKnight.entity.level.painters;
using BurningKnight.entity.level.save;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.secret {
	public class HeartRoomDef : SecretRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(70)) {
				if (Random.Chance(50))
					Painter.Fill(Level, this, 2, Terrain.RandomFloor());
				else
					Painter.FillEllipse(Level, this, 2, Terrain.RandomFloor());
			}

			for (var I = 0; I < Random.NewInt(3, 10); I++) {
				var Point = GetRandomFreeCell();
				var Holder = new HeartFx();
				Holder.X = Point.X * 16 + 3;
				Holder.Y = Point.Y * 16;
				Dungeon.Area.Add(Holder);
				LevelSave.Add(Holder);
			}

			AddEnemies();
		}
	}
}