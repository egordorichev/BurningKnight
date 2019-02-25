using BurningKnight.core.entity.level.painters;
using BurningKnight.core.util;

namespace BurningKnight.core.entity.level.rooms.shop {
	public class GiantShop : ShopRoom {
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
			for (int I = 0; I < Items.Size(); I++) {
				PlaceItem(Items.Get(I), (this.Left + I % (Items.Size() / 3) * 2 + 2) * 16 + 1, (this.Top + 3 + (int) Math.Floor(I / (Items.Size() / 3)) * 2) * 16 - 4);
			}
		}

		public override int GetMinHeight() {
			return 10;
		}

		public override int GetMaxHeight() {
			return 11;
		}

		protected override int GetItemCount() {
			return base.GetItemCount() * 3;
		}
	}
}
