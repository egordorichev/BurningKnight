using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.rooms.shop {
	public class DoubleShop : ShopRoom {
		private bool Vertical;

		public DoubleShop() {
			Vertical = Random.Chance(50);
		}

		public override Void Paint(Level Level) {
			base.Paint(Level);

			if (Random.Chance(50)) {
				Painter.Fill(Level, this, 2, Terrain.RandomFloor());
			} else {
				Painter.FillEllipse(Level, this, 2, Terrain.RandomFloor());
			}


			PlaceItems();
		}

		protected Void PlaceItems(List Items) {
			PlaceItem(Items.Get(0), (this.Left + 2) * 16, (this.Top + 2) * 16);
			PlaceItem(Items.Get(1), (this.Left + (Vertical ? 2 : 4)) * 16, (this.Top + (Vertical ? 4 : 2)) * 16);
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
