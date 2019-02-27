using BurningKnight.entity.level.painters;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.shop {
	public class DoubleShop : ShopRoomDef {
		private bool Vertical;

		public DoubleShop() {
			Vertical = Random.Chance(50);
		}

		public override void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(50))
				Painter.Fill(Level, this, 2, Terrain.RandomFloor());
			else
				Painter.FillEllipse(Level, this, 2, Terrain.RandomFloor());


			PlaceItems();
		}

		protected void PlaceItems(List Items) {
			PlaceItem(Items.Get(0), (Left + 2) * 16, (Top + 2) * 16);
			PlaceItem(Items.Get(1), (Left + (Vertical ? 2 : 4)) * 16, (Top + (Vertical ? 4 : 2)) * 16);
		}

		public override int GetMinWidth() {
			return Vertical ? 5 : 7;
		}

		public override int GetMaxWidth() {
			return Vertical ? 6 : 8;
		}

		public override int GetMinHeight() {
			return Vertical ? 7 : 5;
		}

		public override int GetMaxHeight() {
			return Vertical ? 8 : 6;
		}

		protected override int GetItemCount() {
			return 2;
		}
	}
}