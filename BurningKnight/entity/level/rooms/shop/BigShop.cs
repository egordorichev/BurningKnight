using BurningKnight.entity.level.painters;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.shop {
	public class BigShop : ShopRoomDef {
		public override void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(50))
				Painter.Fill(Level, this, 2, Terrain.RandomFloor());
			else
				Painter.FillEllipse(Level, this, 2, Terrain.RandomFloor());


			PlaceItems();
		}

		protected void PlaceItems(List Items) {
			for (var I = 0; I < Items.Size(); I++) PlaceItem(Items.Get(I), (Left + I % (Items.Size() / 2) * 2 + 2) * 16 + 1, (Top + 3 + (int) Math.Floor(I / (Items.Size() / 2)) * 2) * 16 - 4);
		}

		protected override int GetItemCount() {
			return base.GetItemCount() * 2;
		}
	}
}