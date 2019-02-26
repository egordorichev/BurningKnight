using BurningKnight.entity.item;
using BurningKnight.entity.level.painters;
using BurningKnight.entity.level.save;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.secret {
	public class BombRoom : SecretRoom {
		public override void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(50)) {
				Painter.Fill(Level, this, 1, Random.Chance(50) ? Terrain.FLOOR_D : Terrain.RandomFloor());
				Painter.Fill(Level, this, 2, Terrain.CHASM);
			}
			else {
				Painter.Fill(Level, this, 1, Terrain.CHASM);
			}


			if (Random.Chance(50)) {
				Painter.Fill(Level, this, 3, Terrain.RandomFloor());
				Painter.Fill(Level, this, 4, Terrain.RandomFloor());
			}
			else {
				Painter.FillEllipse(Level, this, 3, Terrain.RandomFloor());
				Painter.FillEllipse(Level, this, 4, Terrain.RandomFloor());
			}


			PaintTunnel(Level, Terrain.FLOOR_D);

			for (var I = 0; I < Random.NewInt(3, 5); I++) {
				var Point = GetCenter();
				var Holder = new ItemHolder(new Bomb());
				Holder.X = Point.X * 16 + 3;
				Holder.Y = Point.Y * 16;
				Dungeon.Area.Add(Holder);
				LevelSave.Add(Holder);
			}
		}

		protected override Point GetDoorCenter() {
			return GetCenter();
		}
	}
}