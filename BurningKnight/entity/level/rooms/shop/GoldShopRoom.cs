using BurningKnight.entity.item;
using BurningKnight.entity.level.painters;
using BurningKnight.entity.level.save;
using BurningKnight.util;
using BurningKnight.util.geometry;

namespace BurningKnight.entity.level.rooms.shop {
	public class GoldShopRoom : ShopRoom {
		public override void Paint(Level Level) {
			base.Paint(Level);
			var El = Random.Chance(50);

			if (El)
				Painter.FillEllipse(Level, this, 1, Terrain.FLOOR_D);
			else
				Painter.Fill(Level, this, 1, Terrain.FLOOR_D);


			El = El || Random.Chance(50);

			if (El)
				Painter.FillEllipse(Level, this, 2, Terrain.RandomFloor());
			else
				Painter.Fill(Level, this, 2, Terrain.RandomFloor());


			El = El || Random.Chance(50);

			if (El)
				Painter.FillEllipse(Level, this, 3, Random.Chance(40) ? Terrain.FLOOR_D : Terrain.RandomFloor());
			else
				Painter.Fill(Level, this, 3, Random.Chance(40) ? Terrain.FLOOR_D : Terrain.RandomFloor());


			Painter.DrawLine(Level, new Point(Left + 1, Bottom - 2), new Point(Right - 1, Bottom - 2), Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA);

			for (var I = 0; I < Random.NewInt(3, 8); I++) {
				var Holder = new ItemHolder(new Gold());
				Holder.GetItem().Generate();
				Holder.X = Left * 16 + 16 + Random.NewInt(GetWidth() * 16 - 32);
				Holder.Y = (Bottom - 1) * 16;
				Dungeon.Area.Add(Holder);
				LevelSave.Add(Holder);
			}

			PlaceItems();
		}

		protected override Point GetSpawn() {
			Point Point;

			do {
				Point = GetRandomFreeCell();
			} while (Point.Y >= Bottom - 2);

			return Point;
		}

		public override int GetMinHeight() {
			return 10;
		}

		public override int GetMaxHeight() {
			return 11;
		}

		public override bool CanConnect(Point P) {
			if (P.Y == Top || P.Y >= Bottom - 2) return false;

			return base.CanConnect(P);
		}
	}
}