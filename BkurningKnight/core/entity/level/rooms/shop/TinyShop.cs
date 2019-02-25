using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.rooms.shop {
	public class TinyShop : ShopRoom {
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
			PlaceItem(Items.Get(0), (this.Left + this.GetWidth() / 2) * 16, (this.Top + this.GetHeight() / 2) * 16);
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
