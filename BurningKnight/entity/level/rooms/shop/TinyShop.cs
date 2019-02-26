using BurningKnight.entity.level.painters;
using BurningKnight.util;

namespace BurningKnight.entity.level.rooms.shop {
	public class TinyShop : ShopRoom {
		public override void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(50))
				Painter.Fill(Level, this, 2, Terrain.RandomFloor());
			else
				Painter.FillEllipse(Level, this, 2, Terrain.RandomFloor());


			PlaceItems();
		}

		protected void PlaceItems(List Items) {
			PlaceItem(Items.Get(0), (Left + GetWidth() / 2) * 16, (Top + GetHeight() / 2) * 16);
		}

		protected override bool Quad() {
			return true;
		}

		public override int GetMinHeight() {
			return 5;
		}

		public override int GetMinWidth() {
			return 5;
		}

		public override int GetMaxWidth() {
			return 8;
		}

		public override int GetMaxHeight() {
			return 8;
		}

		protected override int GetItemCount() {
			return 1;
		}
	}
}