using BurningKnight.core.entity.item;
using BurningKnight.core.entity.level.painters;
using BurningKnight.core.entity.level.save;
using BurningKnight.core.util;
using BurningKnight.core.util.geometry;

namespace BurningKnight.core.entity.level.rooms.shop {
	public class GoldShopRoom : ShopRoom {
		public override Void Paint(Level Level) {
			base.Paint(Level);
			bool El = Random.Chance(50);

			if (El) {
				Painter.FillEllipse(Level, this, 1, Terrain.FLOOR_D);
			} else {
				Painter.Fill(Level, this, 1, Terrain.FLOOR_D);
			}


			El = El || Random.Chance(50);

			if (El) {
				Painter.FillEllipse(Level, this, 2, Terrain.RandomFloor());
			} else {
				Painter.Fill(Level, this, 2, Terrain.RandomFloor());
			}


			El = El || Random.Chance(50);

			if (El) {
				Painter.FillEllipse(Level, this, 3, Random.Chance(40) ? Terrain.FLOOR_D : Terrain.RandomFloor());
			} else {
				Painter.Fill(Level, this, 3, Random.Chance(40) ? Terrain.FLOOR_D : Terrain.RandomFloor());
			}


			Painter.DrawLine(Level, new Point(this.Left + 1, this.Bottom - 2), new Point(this.Right - 1, this.Bottom - 2), Random.Chance(50) ? Terrain.CHASM : Terrain.LAVA);

			for (int I = 0; I < Random.NewInt(3, 8); I++) {
				ItemHolder Holder = new ItemHolder(new Gold());
				Holder.GetItem().Generate();
				Holder.X = this.Left * 16 + 16 + Random.NewInt(this.GetWidth() * 16 - 32);
				Holder.Y = (this.Bottom - 1) * 16;
				Dungeon.Area.Add(Holder);
				LevelSave.Add(Holder);
			}

			PlaceItems();
		}

		protected override Point GetSpawn() {
			Point Point;

			do {
				Point = this.GetRandomFreeCell();
			} while (Point.Y >= this.Bottom - 2);

			return Point;
		}

		public override int GetMinHeight() {
			return 10;
		}

		public override int GetMaxHeight() {
			return 11;
		}

		public override bool CanConnect(Point P) {
			if (P.Y == this.Top || P.Y >= this.Bottom - 2) {
				return false;
			} 

			return base.CanConnect(P);
		}
	}
}
