using BurningKnight.entity.level.painters;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.shop {
	public class ClassicShopRoomDef : ShopRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);
			Painter.Fill(Level, this, 1, Terrain.RandomFloor());

			if (Random.Chance(70)) {
				if (Random.Chance(50))
					Painter.Fill(Level, this, 2, Terrain.RandomFloor());
				else
					Painter.FillEllipse(Level, this, 2, Terrain.RandomFloor());
			}

			PlaceItems();
		}
	}
}